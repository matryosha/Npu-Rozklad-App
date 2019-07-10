from colorama import Fore, Style


def info(message):
    print(Fore.BLUE + Style.DIM + '[i] ', end='')
    print(Fore.BLUE + message)


def error(message):
    print(Fore.RED + Style.DIM + '[e] ', end='')
    print(Fore.RED + message)