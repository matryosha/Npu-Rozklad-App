import argparse

from colorama import init


class RunArguments:
    up = False
    push = False
    publish = False
    configuration: str


def parse_args():
    parser = argparse.ArgumentParser(description='Builds rozklad-app')
    parser.add_argument('--up', help='Building and compose up project', action='store_true')
    parser.add_argument('--push', help='Building and push to docker hub', action='store_true')
    parser.add_argument(
        '--publish', help='Building and publishing app with selected configuration', action='store_true')
    parser.add_argument('--configuration', help='Building configuration', default='Development')

    args = parser.parse_args()
    arguments = RunArguments()
    arguments.up = args.up
    arguments.push = args.push
    arguments.publish = args.publish
    arguments.configuration = args.configuration
    return arguments


init(autoreset=True)