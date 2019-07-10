import utilities.log as log


def handle_called_process_error(error, message):
    log.error(message)
    log.error("stdout:")
    log.error(str(error.stdout.decode('utf-8')))
    log.error("stderr:")
    log.error(str(error.stderr.decode('utf-8')))
    exit(1)

