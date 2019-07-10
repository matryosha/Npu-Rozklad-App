import os
import yaml


def load():
    with open(os.path.join(os.path.abspath(os.curdir), 'config.yaml')) as config_file:
        content = config_file.read()
        return yaml.load(content, Loader=yaml.BaseLoader)

