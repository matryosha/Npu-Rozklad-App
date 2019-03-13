import os
from shutil import copyfile

from cement import Controller, ex
from cement.utils import fs


class Scenario(Controller):
    class Meta:
        label = 'scenario'
        stacked_on = 'base'
        stacked_type = 'nested'
        description = 'managing scenario'
        help = 'managing scenario'

    def get_scenario_path(self, scenario_name):
        return fs.abspath(self.app.config.get(
            'aspdepy', 'default_path') + 'scenarios/' + scenario_name + '.ascenario')

    def _default(self):
        """Default action if no sub-command is passed."""
        self.list()

    @ex(
        help='list of added scenarios'
    )
    def list(self):
        print('Scenarios: ')
        scenarios_path = fs.abspath(self.app.config.get('aspdepy', 'default_path') + 'scenarios')

        for scenario in os.listdir(scenarios_path):
            print('* %s' % os.path.splitext(scenario)[0])

    @ex(
        help='add scenario',
        arguments=[
            (['-p', '--path'],
             {'help': 'path to scenario',
              'action': 'store',
              'dest': 'scenario_path'}),
            (['-n', '--name'],
             {'help': 'specify name',
              'action': 'store',
              'dest': 'scenario_name'}),
        ],
    )
    def add(self):
        scenario_path = self.app.pargs.scenario_path
        scenario_name = self.app.pargs.scenario_name
        if scenario_path is None:
            print('path must me specified')
            return

        if not os.path.isfile(scenario_path):
            print('wrong file')
            return

        scenario_def_path = fs.abspath(self.app.config.get(
            'aspdepy', 'default_path') + 'scenarios/' + scenario_name + '.ascenario')
        copyfile(scenario_path, scenario_def_path)

        print('Copied %s' % scenario_name)

    @ex(
        help='delete scenario',
        arguments=[
            (['-n', '--name'],
             {'help': 'delete scenario with given name',
              'action': 'store',
              'dest': 'scenario_name'}),
        ],
    )
    def delete(self):
        scenario_path = self.get_scenario_path(self.app.pargs.scenario_name)

        if not os.path.exists(scenario_path):
            print('Cant find scenario with given name')
            return

        os.remove(scenario_path)

        print('Deleted %s' % self.app.pargs.scenario_name)
