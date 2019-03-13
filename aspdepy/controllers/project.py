import os

from cement import Controller, ex
from tinydb import Query, operations


class Project(Controller):
    class Meta:
        label = 'project'
        stacked_on = 'base'
        stacked_type = 'nested'
        description = 'managing projects'
        help = 'managing projects'

    def _default(self):
        """Default action if no sub-command is passed."""

        self.list()

    @staticmethod
    def get_project(app, project_name):
        projects_table = app.db.table('projects')

        query = Query()
        return next(iter(projects_table.search(query.name == project_name)), None)

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

        project_query = Query()
        if projects_table.search(project_query.name == project_name):
            print('Project with that name already exist')
            return

        # new_project = next(iter(projects_table.search(project_query.path == project_path)), None)
        # if new_project is not None:
        #     print('This project already exist with name %s' % new_project['name'])
        #     return

        projects_table.insert({'path': project_path,
                               'name': project_name})
        print('added %s' % project_name)

    @ex(
        help='list of added projects'
    )
    def list(self):
        print('Added projects: ')
        for row in self.app.db.table('projects'):
            print('* - [%s] %s@%s' %(row['name'], row['username'], row['server_ip']))

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
        project = self.get_project(self.app.pargs.project_name)

        if project is None:
            print('There is no project with given name')
            return

        self.app.db.table('projects').remove(doc_ids=[project.doc_id])

        print('Deleted')

    @ex(
        help='Set ssh user for given project',
        arguments=[
            (['-n', '--name'],
             {'help': 'name of project',
              'action': 'store',
              'dest': 'project_name'}),
            (['-u', '--username'],
             {'help': 'name of user',
              'action': 'store',
              'dest': 'user_name'})
        ],
    )
    def setuser(self):
        projects_table = self.app.db.table('projects')

        query = Query()
        project = Project.get_project(self.app, self.app.pargs.project_name)
        if project is None:
            print('There is no project with given name')
            return

        projects_table.update(operations.set(
            'username', self.app.pargs.user_name),
            query.name == self.app.pargs.project_name)

    @ex(
        help='Set remote server ip for given project',
        arguments=[
            (['-n', '--name'],
             {'help': 'name of project',
              'action': 'store',
              'dest': 'project_name'}),
            (['-s', '--server-ip'],
             {'help': 'sever-ip',
              'action': 'store',
              'dest': 'server_ip'})
        ],
    )
    def setserver(self):
        projects_table = self.app.db.table('projects')
        query = Query()

        if Project.get_project(self.app, self.app.pargs.project_name) is not None:
            print('There is no project with given name')
            return

        projects_table.update(operations.set(
            'server_ip', self.app.pargs.server_ip),
            query.name == self.app.pargs.project_name)

