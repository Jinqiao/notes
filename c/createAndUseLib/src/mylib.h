#ifndef _MYLIB_H_
#define _MYLIB_H_

// a constant definition exported by library:
#define MAX_FOO  20

// a type definition exported by library:
struct foo_struct {
  int x;
  float y;
};

typedef struct foo_struct foo_struct;


// "extern" means that this is a variable declaration but not a defination
// See K&R Chapter 4.4
extern int total_foo;

// function prototypes exported by library:
extern double my_add(double y, double z);
extern double my_minus(double y, double z);

#endif
