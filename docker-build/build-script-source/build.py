import os
import shutil
import subprocess
import utilities.log as log
import utilities.docker_wrapper as docker
import utilities.config_loader as config_loader
from utilities.docker_wrapper import DockerArgument, DockerArgumentType
from utilities.cli import parse_args
from subprocess import CalledProcessError

from utilities.exceptions_handle import handle_called_process_error

# project_path = '/media/matryoshka/Stuff/Docs/Projects/Sharp/RozkladNpuBot'
#
# webapi_project_path = os.path.join(project_path, 'RozkladNpuBot.WebApi')
# script_work_dir = os.path.join(project_path, 'docker-build')
# environment_files_path = os.path.join(script_work_dir, 'env-files')
# nginx_certs_path = os.path.join(script_work_dir, 'nginx', 'nginx-certs')
#
# webapi_output_folder_name = 'release'
# dotnet_configuration = 'Docker'
project_path: str

webapi_project_path: str
script_work_dir: str
environment_files_path: str
nginx_certs_path: str

webapi_output_folder_name: str
dotnet_configuration: str


def publish_dotnet_app():
    webapi_output_folder = os.path.join(webapi_project_path, webapi_output_folder_name)
    if os.path.isdir(webapi_output_folder):
        shutil.rmtree(webapi_output_folder)
    log.info('Publishing app...')
    log.info(f'Command: dotnet publish {webapi_project_path} -c {dotnet_configuration} -o {webapi_output_folder}\n')
    try:
        subprocess.run(
            ['dotnet', 'publish', webapi_project_path, '-c', dotnet_configuration, '-o', webapi_output_folder],
            capture_output=True, check=True)
        log.info('Done publishing\n')
    except CalledProcessError as error:
        handle_called_process_error(error, 'Error when publishing dotnet app:')


def check_docker_env_files_existence():
    log.info("Checking env files existence...\n")
    if not os.path.isfile(os.path.join(environment_files_path, 'mysql-db.env')):
        log.error("Environment file for mysql does not exist")
        exit(1)
    if not os.path.isfile(os.path.join(environment_files_path, 'mongo.env')):
        log.error("Environment file for mongo does not exist")
        exit(1)


def get_multiple_compose_file_argument():
    docker_compose_path = os.path.join(script_work_dir, 'docker-compose.yml')
    docker_compose_build_paths_path = os.path.join(script_work_dir, 'docker-compose.build-paths.yml')
    docker_compose_current_conf_path = os.path.join(script_work_dir,
                                                    f'docker-compose.{__run_arguments__.configuration}.yml')
    return f'-f {docker_compose_path} ' \
        f'-f {docker_compose_build_paths_path} ' \
        f'-f {docker_compose_current_conf_path}'


def docker_compose_build():
    log.info("Docker compose build...\n")
    try:
        docker.call_docker_compose([
            DockerArgument(type=DockerArgumentType.SPLIT,
                           value=f"{compose_files_arg} build")
        ], capture_output=True, check=True)
    except CalledProcessError as error:
        handle_called_process_error(error, 'Error when docker-compose build:')


def docker_compose_push():
    log.info("Docker compose push...\n")
    try:
        docker.call_docker_compose([
            DockerArgument(type=DockerArgumentType.SPLIT,
                           value=f"{compose_files_arg} push")
        ], capture_output=False, check=True)
    except CalledProcessError as error:
        handle_called_process_error(error, 'Error when trying docker-compose push:')


def docker_compose_up():
    try:
        docker.call_docker_compose([
            DockerArgument(type=DockerArgumentType.SPLIT,
                           value=f"{compose_files_arg} up")
        ], capture_output=False)
    except CalledProcessError as error:
        handle_called_process_error(error, "Error when trying docker-compose up :")


def publish():
    log.info("Publishing...")
    cur_dir_path = script_work_dir

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
        log.error(f'File secret.{dotnet_configuration}.json does not exist')
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
    log.info("Done!")


def set_configuration_vars(configurations):
    global project_path, webapi_output_folder_name, dotnet_configuration, webapi_project_path, \
        script_work_dir, environment_files_path, nginx_certs_path

    log.info("Loading configuration...")
    project_path = configurations['project_path']
    webapi_output_folder_name = configurations['webapi_output_folder_name']
    dotnet_configuration = configurations['dotnet_configuration']

    webapi_project_path = os.path.join(project_path, 'RozkladNpuBot.WebApi')
    script_work_dir = os.path.join(project_path, 'docker-build')
    environment_files_path = os.path.join(script_work_dir, 'env-files')
    nginx_certs_path = os.path.join(script_work_dir, 'nginx', 'nginx-certs')


if __name__ == '__main__':
    set_configuration_vars(config_loader.load())
    __run_arguments__ = parse_args()

    if __run_arguments__.configuration != 'Production':
        dotnet_configuration = 'DockerDevel'

    publish_dotnet_app()

    check_docker_env_files_existence()

    if __run_arguments__.configuration == 'Development':
        docker.recreate_dev_volumes(webapi_project_path, nginx_certs_path, dotnet_configuration)

    compose_files_arg = get_multiple_compose_file_argument()

    docker_compose_build()

    if __run_arguments__.push is True:
        docker_compose_push()

    if __run_arguments__.publish is True:
        publish()

    if __run_arguments__.up is True:
        docker_compose_up()
