# loop through command args
for arg do
    echo arg is $arg
done

# loop through list
for i in 1 2 3
do
   echo "Welcome $i times"
done

for i in {0..10..2}
do 
    echo "Hello $i times"
done

# three expression for loop
for (( c=1; c<=5; c++ ))
do  
    echo "ok $c times"
done
