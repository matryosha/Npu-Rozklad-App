import argparse
import os
import shutil
import subprocess
import shlex
from enum import Enum
from subprocess import CalledProcessError
from typing import List
import glob

from colorama import init
from colorama import Fore, Style

webapi_project_path = '/media/matryoshka/Stuff/Docs/Projects/Sharp/RozkladNpuBot/RozkladNpuBot.WebApi/'
# webapi_project_path = os.path.abspath('../RozkladNpuBot.WebApi')
webapi_output_folder_name = 'release'
dotnet_configuration = 'Docker'
script_work_dir = '/media/matryoshka/Stuff/Docs/Projects/Sharp/RozkladNpuBot/docker-build'
environment_files_path = "/media/matryoshka/Stuff/Docs/Projects/Sharp/RozkladNpuBot/docker-build/env-files"
nginx_certs_path = '/media/matryoshka/Stuff/Docs/Projects/Sharp/RozkladNpuBot/docker-build/nginx/nginx-certs'


# environment_files_path = os.path.abspath('./env-files')

def log_info(message):
    print(Fore.BLUE + Style.DIM + '[i] ', end='')
    print(Fore.BLUE + message)


def log_error(message):
    print(Fore.RED + Style.DIM + '[e] ', end='')
    print(Fore.RED + message)


def parse_args(arguments):
    """

    :type arguments: RunArguments
    """
    parser = argparse.ArgumentParser(description='Builds rozklad-app')
    parser.add_argument('--up', help='Building and compose up project', action='store_true')
    parser.add_argument('--push', help='Building and push to docker hub', action='store_true')
    parser.add_argument(
        '--publish', help='Building and publishing app with selected configuration', action='store_true')
    parser.add_argument('--configuration', help='Building configuration', default='Development')

    args = parser.parse_args()
    arguments.up = args.up
    arguments.push = args.push
    arguments.publish = args.publish
    arguments.configuration = args.configuration


class RunArguments:
    up = False
    push = False
    publish = False
    configuration: str


class DockerArgumentType(Enum):
    SPLIT = 1
    NO_SPLIT = 2


class DockerArgument:
    def __init__(self, type: DockerArgumentType, value: str):
        self.type = type
        self.value = value

    type: DockerArgumentType
    value: str


def get_args_list_from_docker_arguments(docker_arguments: List[DockerArgument]):
    result = []
    for docker_argument in docker_arguments:
        result.extend(extract_docker_argument(docker_argument))
    return result


def extract_docker_argument(argument: DockerArgument):
    if argument.type == DockerArgumentType.SPLIT:
        return argument.value.split(' ')
    if argument.type == DockerArgumentType.NO_SPLIT:
        return [argument.value]
    raise NotImplementedError


def call_docker_compose(docker_arguments: List[DockerArgument], **subprocess_kargs):
    result_command = ["docker-compose"]
    result_command.extend(get_args_list_from_docker_arguments(docker_arguments))
    return subprocess.run(result_command, **subprocess_kargs)


def call_docker(docker_arguments: List[DockerArgument], **subprocess_kargs):
    result_command = ["docker"]
    result_command.extend(get_args_list_from_docker_arguments(docker_arguments))
    return subprocess.run(result_command, **subprocess_kargs)


def handle_called_process_error(error):
    log_error("Error when docker-compose build:")
    log_error("stdout:")
    log_error(str(error.stdout.decode('utf-8')))
    log_error("stderr:")
    log_error(str(error.stderr.decode('utf-8')))
    exit(1)

def recreate_dev_volumes():
    dev_rozklad_app_volume_name = 'rozklad-app-dev-secrets'
    dev_nginx_certs_volume_name = 'rozklad-nginx-dev-certs'

    log_info("Recreating and populating volumes...")

    log_info("Removing existed volumes")
    subprocess.run(
        ['docker', 'volume', 'rm', 'rozklad-app-dev-secrets', 'rozklad-nginx-dev-certs'],
        capture_output=True)

    log_info("Populating rozklad-app volume...")
    try:
        rozklad_docker_commands = f'docker run --rm -v {dev_rozklad_app_volume_name}:/volume ' \
            f'-v {webapi_project_path}/Properties:/properties ' \
            f'ubuntu bash -c'.split(' ')
        rozklad_docker_commands.append('"cp /properties/secret.*.json /volume"')
        subprocess.run(rozklad_docker_commands, capture_output=True, check=True)

    except CalledProcessError as error:
        log_error("Error when population rozklad volume:")
        log_error(str(error.stderr.decode('utf-8')))
        exit(1)

    log_info("Populating rozklad-nginx volume...")
    nginx_docker_commands = f'docker run --rm -v {dev_nginx_certs_volume_name}:/volume ' \
        f'-v {nginx_certs_path}/Development:/certs ' \
        f'ubuntu bash -c'.split(' ')
    nginx_docker_commands.append('"cp /certs/* /volume"')
    subprocess.run(nginx_docker_commands, capture_output=True, check=True)


if __name__ == '__main__':
    init(autoreset=True)
    __run_arguments__ = RunArguments()
    parse_args(__run_arguments__)
    if __run_arguments__.configuration != 'Production':
        dotnet_configuration = 'DockerDevel'

    webapi_output_folder = os.path.join(webapi_project_path, webapi_output_folder_name)

    if os.path.isdir(webapi_output_folder):
        log_info('Removing old release folder')
        shutil.rmtree(webapi_output_folder)

    log_info('Publishing app...')
    log_info(f'Command: dotnet publish {webapi_project_path} -c {dotnet_configuration} -o {webapi_output_folder}\n')
    try:
        dotnet_process = \
            subprocess.run(
                ['dotnet', 'publish', webapi_project_path, '-c', dotnet_configuration, '-o', webapi_output_folder],
                capture_output=True, check=True)
        log_info('Done publishing\n')
    except CalledProcessError as error:
        log_error("Error when publishing app:")
        log_error(str(error.stdout.decode('utf-8')))
        exit(1)

    log_info("Checking env files existence...\n")
    if not os.path.isfile(os.path.join(environment_files_path, 'mysql-db.env')):
        log_error("Environment file for mysql does not exist")
        exit(1)
    if not os.path.isfile(os.path.join(environment_files_path, 'mongo.env')):
        log_error("Environment file for mongo does not exist")
        exit(1)

    if __run_arguments__.configuration == 'Development':
        recreate_dev_volumes()

    docker_compose_path = os.path.join(script_work_dir, 'docker-compose.yml')
    docker_compose_build_paths_path = os.path.join(script_work_dir, 'docker-compose.build-paths.yml')
    docker_compose_current_conf_path = os.path.join(script_work_dir,
                                                    f'docker-compose.{__run_arguments__.configuration}.yml')
    compose_files_arg = f'-f {docker_compose_path} ' \
        f'-f {docker_compose_build_paths_path} ' \
        f'-f {docker_compose_current_conf_path}'

    log_info("Docker compose build...\n")
    try:
        call_docker_compose([
            DockerArgument(type=DockerArgumentType.SPLIT,
                           value=f"{compose_files_arg} build")
        ], capture_output=True, check=True)
    except CalledProcessError as error:
        handle_called_process_error(error)

    if __run_arguments__.push is True:
        log_info("Docker compose push...\n")
        try:
            call_docker_compose([
                DockerArgument(type=DockerArgumentType.SPLIT,
                               value=f"{compose_files_arg} push")
            ], capture_output=False, check=True)
        except CalledProcessError as error:
            handle_called_process_error(error)

    if __run_arguments__.up is True:
        try:
            call_docker_compose([
                DockerArgument(type=DockerArgumentType.SPLIT,
                               value=f"{compose_files_arg} up")
            ], capture_output=True, check=True)
        except CalledProcessError as error:
            handle_called_process_error(error)

    if __run_arguments__.publish is True:
        log_info("Publishing...")
        cur_dir_path = script_work_dir
        # cur_dir_path = os.path.abspath(os.path.curdir)
        publish_path = os.path.join(cur_dir_path, 'publish')

        if os.path.isdir(publish_path):
            shutil.rmtree(publish_path)

        os.mkdir(publish_path)
        rozklad_app_publish_folder_name = 'rozklad-app-secrets'
        nginx_certs_publish_folder_name = 'rozklad-app-nginx-certs'
        os.mkdir(os.path.join(publish_path, rozklad_app_publish_folder_name))
        os.mkdir(os.path.join(publish_path, nginx_certs_publish_folder_name))

        secret_file_path = os.path.join(webapi_project_path, 'Properties', f'secret.{dotnet_configuration}.json')
        if not os.path.isfile(secret_file_path):
            log_error(f'File secret.{dotnet_configuration}.json does not exist')
            exit(1)

        shutil.copy(secret_file_path, os.path.join(publish_path, rozklad_app_publish_folder_name))

        os.mkdir(os.path.join(publish_path, 'env-files'))
        for file in os.listdir(environment_files_path):
            shutil.copy(os.path.join(environment_files_path, file), os.path.join(publish_path, 'env-files'))

        shutil.copy(os.path.join(cur_dir_path, 'docker-compose.yml'), publish_path)
        shutil.copyfile(os.path.join(cur_dir_path, f'docker-compose.{__run_arguments__.configuration}.yml'),
                        os.path.join(publish_path, 'docker-compose.override.yml'))

        current_conf_nginx_certs_path = os.path.join(nginx_certs_path, __run_arguments__.configuration)
        for file in os.listdir(current_conf_nginx_certs_path):
            shutil.copy(os.path.join(current_conf_nginx_certs_path, file),
                        os.path.join(publish_path, nginx_certs_publish_folder_name))

        shutil.copy(os.path.join(cur_dir_path, 'prepare-host.sh'), publish_path)