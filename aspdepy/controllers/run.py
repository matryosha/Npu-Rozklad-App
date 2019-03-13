import os
import shutil
import subprocess
import sys
import tarfile
from getpass import getpass

import paramiko
from cement import Controller, ex
from cement.utils import fs
from scp import SCPClient

from aspdepy.controllers.project import Project
from aspdepy.controllers.scenario import Scenario


def progress(filename, size, sent):
    sys.stdout.write("%s\'s progress: %.2f%%   \r" % (filename, float(sent)/float(size)*100) )


class Run(Controller):
    class Meta:
        label = 'exec'
        stacked_on = 'base'
        stacked_type = 'nested'
        description = 'run scenarios'
        help = 'run scenarios'

    @ex(
        help='run run run',
        arguments=[
            (['-p', '--project-name'],
             {'help': 'project name',
              'action': 'store',
              'dest': 'project_name'}),
            (['-s', '--scenario-name'],
             {'help': 'scenario name',
              'action': 'store',
              'dest': 'scenario_name'}),
        ],
    )
    def run(self):
        scenario_path = Scenario.get_scenario_path(
            self.app.pargs.scenario_name
        )

    @ex(
        help="build project, send to server and execute scenario's commands",
        arguments=[
            (['-p', '--project-name'],
             {'help': 'project name',
              'action': 'store',
              'dest': 'project_name'}),
            (['-s', '--scenario-name'],
             {'help': 'scenario name',
              'action': 'store',
              'dest': 'scenario_name'}),
        ],
    )
    def deploy(self):
        scenario = Scenario.get_scenario_path(
            app=self.app,
            scenario_name=self.app.pargs.scenario_name)
        project = Project.get_project(
            app=self.app,
            project_name=self.app.pargs.project_name)

        aspdepy_path = fs.abspath(self.app.config.get('aspdepy', 'default_path'))
        temp_publish_path = fs.abspath(aspdepy_path + '/temp_publish')
        print("Running dotnet...")
        completed_process = subprocess.run(
            "dotnet publish %s -c Release -o %s" % (project['path'], temp_publish_path), capture_output=True)
        print("dotnet output:")
        print(completed_process.stdout.decode("utf-8"))
        if completed_process.returncode is not 0:
            print("dotnet return non 0 code. Finishing...")
            return
        # todo extract errors and warnings using regex

        with tarfile.open(fs.abspath(aspdepy_path + "/send.tar"), "w") as tar:
            tar.add(temp_publish_path, arcname='temp_publish')

        pw = getpass("Password for %s@%s: " % (project['username'], project['server_ip']))
        # todo rename project server to hostname
        client = paramiko.SSHClient()
        client.load_system_host_keys()
        # client.set_missing_host_key_policy(paramiko.WarningPolicy())
        client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        client.connect(project['server_ip'], 22, project['username'], pw)

        with SCPClient(client.get_transport(), progress=progress) as scp:
            scp.put(fs.abspath(aspdepy_path + "/send.tar"), remote_path='/')

        # client.exec_command()

        with open(scenario) as scenario:
            scenario_commands = scenario.read().splitlines()

        for command in scenario_commands:
            print("Executing '%s' command" % command)
            stdin, stdout, stderr  = client.exec_command(command)
            if stdout.channel.recv_exit_status() is not 0:
                print("command '%s' returned non zero result" % command)
                print(stderr.read().decode("utf-8"))
                print("Finishing...")
                return
            stdout_text = stdout.read().decode("utf-8")
            print("Command output: ")
            print(stdout_text)

        shutil.rmtree(temp_publish_path)
        os.remove(fs.abspath(aspdepy_path + "/send.tar"))

        print("All done!")

