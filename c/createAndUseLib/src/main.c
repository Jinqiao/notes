#include <stdio.h>
#include <stdlib.h>
#include "mylib.h"

int main(int argc, char** argv){
  if(argc < 3){
    printf("need 2 arguments");
    return 1;
  }

  printf("sum is %.2f\n", my_add(atof(argv[1]), atof(argv[2])));
  printf("diff is %.2f\n", my_minus(atof(argv[1]), atof(argv[2])));

  return 0;
}
