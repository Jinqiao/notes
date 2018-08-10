# 1. The default values are evaluated at the point of function definition in
#  the `defining' scope.

i = 5


def f(arg=i):
    print(arg)


i = 6
f()  # print '5'


def f(a, L=[]):
    L.append(a)
    return L


print(f(1))  # => [1]
print(f(2))  # => [1, 2]
print(f(3))  # => [1, 2, 3]


# 2. *tuple and **dict style function arguments
def cheeseshop(kind, *arguments, **keywords):
    print("-- Do you have any", kind, "?")
    print("-- I'm sorry, we're all out of", kind)
    for arg in arguments:
        print(arg)
    print("-" * 40)
    for kw in keywords:
        print(kw, ":", keywords[kw])


# 3. Upacking Argument lists, using * for list and ** for dict
args = [3, 6]
list(range(*args))  # this is <=> list(range(3, 6))


def parrot(voltage, state='a stiff', action='voom'):
    print("-- This parrot wouldn't", action)
    print("if you put", voltage, "volts through it.")
    print("E's", state, "!")


d = {"voltage": "four million", "state": "bleedin' demised", "action": "VOOM"}
parrot(**d)


# 4. Methods like ‘insert’, ‘remove’ or ‘sort’ that only modify the list have
# no return value – they return the default ‘None’. This is a design principle
# for all mutable data structures in Python.


# 5. List Comprehension
[(x, y) for x in [1, 2, 3] for y in [3, 1, 4] if x != y]

# is equivalent to
combs = []
for x in [1, 2, 3]:
    for y in [3, 1, 4]:
        if x != y:
            combs.append((x, y))


# 6. Loop over 2 list at the same time
questions = ['name', 'quest', 'favorite color']
answers = ['lancelot', 'the holy grail', 'blue']
for q, a in zip(questions, answers):
    print('What is your {0}?  It is {1}.'.format(q, a))


# 7. Understand Module
# A module is a file containing Python definitions and statements.  The
# file name is the module name with the suffix ‘.py’ appended.  Within a
# module, the module’s name (as a string) is available as the value of the
# global variable ‘__name__’.


# Note: For efficiency reasons, each module is only imported once per
# interpreter session.  Therefore, if you change your modules, you
# must restart the interpreter – or, if it’s just one module you want
# to test interactively, use importlib.reload(): e.g.
# ‘import importlib; importlib.reload(modulename)’


# When you run a Python module with

#      python fibo.py <arguments>

# the code in the module will be executed, just as if you imported it, but
# with the ‘__name__’ set to ‘"__main__"’.  That means that by adding this
# code at the end of your module:

#      if __name__ == "__main__":
#          import sys
#          fib(int(sys.argv[1]))

# you can make the file usable as a script as well as an importable
# module, because the code that parses the command line only runs if the
# module is executed as the “main” file:


# 8. Understand Package
#      sound/                          Top-level package
#            __init__.py               Initialize the sound package
#            formats/                  Subpackage for file format conversions
#                    __init__.py
#                    wavread.py
#                    wavwrite.py
#                    aiffread.py
#                    aiffwrite.py
#                    auread.py
#                    auwrite.py
#                    ...
#            effects/                  Subpackage for sound effects
#                    __init__.py
#                    echo.py
#                    surround.py
#                    reverse.py
#                    ...
#            filters/                  Subpackage for filters
#                    __init__.py
#                    equalizer.py
#                    vocoder.py
#                    karaoke.py
#                    ...

# When importing the package, Python searches through the directories on
# ‘sys.path’ looking for the package subdirectory.

# The ‘__init__.py’ files are required to make Python treat the
# directories as containing packages; this is done to prevent directories
# with a common name, such as ‘string’, from unintentionally hiding valid
# modules that occur later on the module search path.  In the simplest
# case, ‘__init__.py’ can just be an empty file, but it can also execute
# initialization code for the package or set the ‘__all__’ variable,

# Note that when using ‘from package import item’, the item can be either
# a submodule (or subpackage) of the package, or some other name defined
# in the package, like a function, class or variable.  The ‘import’
# statement first tests whether the item is defined in the package; if
# not, it assumes it is a module and attempts to load it.  If it fails to
# find it, an ImportError exception is raised.

# Contrarily, when using syntax like ‘import item.subitem.subsubitem’,
# each item except for the last must be a package; the last item can be a
# module or a package but can’t be a class or function or variable defined
# in the previous item.


# 9. Importing * From a Package
# The *note import statement uses the following
# convention: if a package’s ‘__init__.py’ code defines a list named
# ‘__all__’, it is taken to be the list of module names that should be
# imported when ‘from package import *’ is encountered. It is up to the
# package author to keep this list up-to-date when a new version of the
# package is released.  Package authors may also decide not to support it,
# if they don’t see a use for importing * from their package.  For
# example, the file ‘sound/effects/__init__.py’ could contain the
# following code:

#      __all__ = ["echo", "surround", "reverse"]

# This would mean that ‘from sound.effects import *’ would import the
# three named submodules of the ‘sound’ package.


# 10. Scope and Namespaces
def scope_test():
    def do_local():
        spam = "local spam"

    def do_nonlocal():
        # not work in python 2
        nonlocal spam
        spam = "nonlocal spam"

    def do_global():
        global spam
        spam = "global spam"
    spam = "test spam"
    do_local()
    print("After local assignment:", spam)
    do_nonlocal()
    print("After nonlocal assignment:", spam)
    do_global()
    print("After global assignment:", spam)


scope_test()
print("In global scope:", spam)
