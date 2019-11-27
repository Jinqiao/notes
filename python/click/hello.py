
# below recommeded click
# https://stackoverflow.com/a/27069329/4499942
# https://realpython.com/comparing-python-command-line-parsing-libraries-argparse-docopt-click/

import click


@click.command()
@click.option('--count', default=1, help='Number of greetings.')
@click.option('--name', prompt='Your name',
              help='The person to greet.')
def hello(count, name):
    """Simple program that greets NAME for a total of COUNT times."""
    for x in range(count):
        click.echo('Hello %s!' % name)


if __name__ == '__main__':
    hello()
