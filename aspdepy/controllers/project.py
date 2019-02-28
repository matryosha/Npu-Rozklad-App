import os

from cement import Controller, ex
from tinydb import Query


class Project(Controller):
    class Meta:
        label = 'project'
        stacked_on = 'base'
        stacked_type = 'nested'
        description = 'managing projects'

    def _default(self):
        """Default action if no sub-command is passed."""

        self.list()

    @ex(
        help='add project',
        arguments=[
            ### add a sample foo option under subcommand namespace
            (['-p', '--path'],
             {'help': 'add project with path',
              'action': 'store',
              'dest': 'project_path'}),
            (['-n', '--name'],
             {'help': 'specify name',
              'action': 'store',
              'dest': 'project_name'}),
        ],
    )
    def add(self):
        project_path = self.app.pargs.project_path
        project_name = self.app.pargs.project_name
        if project_path is None:
            print('path must me specified')
            return

        if not os.path.isfile(project_path):
            print('wrong file')
            return

        projects_table = self.app.db.table('projects')

        project = Query()
        if projects_table.search(project.name == project_name):
            print('Project with that name already exist')
            return

        new_project = next(iter(projects_table.search(project.path == project_path)), None)
        if new_project is not None:
            print('This project already exist with name %s' % new_project['name'])
            return

        projects_table.insert({'path': project_path,
                               'name': project_name})
        print('added %s' % project_name)

    @ex(
        help='list of added projects'
    )
    def list(self):
        print('Added projects: ')
        for row in self.app.db.table('projects'):
            print('* - %s ' % row['name'])

    @ex(
        help='delete project from list',
        arguments=[
            ### add a sample foo option under subcommand namespace
            (['-n', '--name'],
             {'help': 'delete project with given name',
              'action': 'store',
              'dest': 'project_name'}),
        ],
    )
    def delete(self):
        query = Query()

        project = next(iter(self.app.db.table('projects').search(query.name == self.app.pargs.project_name)), None)

        if project is None:
            print('There is no project with given name')
            return

        self.app.db.table('projects').remove(doc_ids=[project.doc_id])

        print('Deleted')
