// to compile, see https://stackoverflow.com/a/14884166
// $ gcc -shared -o libmylib.so -fPIC mylib.c
// See HOWTO file to compile on Windows

double my_add(double a, double b)
{
  return a + b;
}

double my_minus(double a, double b)
{
  return a - b;
}

