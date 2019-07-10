import subprocess
from enum import Enum
from typing import List

import utilities.log as log
from utilities.exceptions_handle import handle_called_process_error


class DockerArgumentType(Enum):
    SPLIT = 1
    NO_SPLIT = 2


class DockerArgument:
    def __init__(self, type: DockerArgumentType, value: str):
        self.type = type
        self.value = value

    type: DockerArgumentType
    value: str


def recreate_dev_volumes(webapi_project_path, nginx_certs_path, dotnet_configuration):
    dev_rozklad_app_volume_name = 'rozklad-app-dev-secrets'
    dev_nginx_certs_volume_name = 'rozklad-nginx-dev-certs'

    log.info("Recreating and populating volumes...")

    log.info("Removing existed volumes")
    call_docker([
        DockerArgument(
            value='volume rm rozklad-app-dev-secrets rozklad-nginx-dev-certs',
            type=DockerArgumentType.SPLIT
        )
    ], capture_output=True)

    log.info("Populating rozklad-app volume...")
    try:
        call_docker([
            DockerArgument(
                value=f'run --rm -v {dev_rozklad_app_volume_name}:/volume '
                f'-v {webapi_project_path}/Properties:/properties '
                f'ubuntu cp /properties/secret.{dotnet_configuration}.json /volume',
                type=DockerArgumentType.SPLIT
            ),
        ], capture_output=True, check=True)
    except subprocess.CalledProcessError as error:
        handle_called_process_error(error, 'Error when populating rozklad volume')

    log.info("Populating rozklad-nginx volume...")
    try:
        call_docker([
            DockerArgument(
                value=f'run --rm -v {dev_nginx_certs_volume_name}:/volume '
                f'-v {nginx_certs_path}/Development:/certs '
                f'ubuntu cp /certs/cert.pem /certs/key.pem /volume',
                type=DockerArgumentType.SPLIT
            )
        ], capture_output=True, check=True)
    except subprocess.CalledProcessError as error:
        handle_called_process_error(error, 'Error when populating nginx volume')


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
