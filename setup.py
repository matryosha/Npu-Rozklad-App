
from setuptools import setup, find_packages
from aspdepy.core.version import get_version

VERSION = get_version()

f = open('README.md', 'r')
LONG_DESCRIPTION = f.read()
f.close()

setup(
    name='aspdepy',
    version=VERSION,
    description='CD for rozkladbot',
    long_description=LONG_DESCRIPTION,
    long_description_content_type='text/markdown',
    author='matryoshka',
    author_email='nazardjali@gmail.com',
    url='https://github.com/johndoe/myapp/',
    license='unlicensed',
    packages=find_packages(exclude=['ez_setup', 'tests*']),
    package_data={'aspdepy': ['templates/*']},
    include_package_data=True,
    entry_points="""
        [console_scripts]
        aspdepy = aspdepy.main:main
    """,
)
