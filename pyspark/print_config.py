
# https://stackoverflow.com/a/46215539/4499942


# Print spark conf
# to get a conf object we can also use: sc.getConf()
# to get a sparkContext we can use the free sc or spark.sparkContext
for item in sorted(sc._conf.getAll()): 
    print(item)


# Print hadoop conf
hadoopConf = {}
iterator = sc._jsc.hadoopConfiguration().iterator()

while iterator.hasNext():
    prop = iterator.next()
    hadoopConf[prop.getKey()] = prop.getValue()

for item in sorted(hadoopConf.items()): 
    print(item)


# Print env
# or just use !env in ipython
import os
for item in sorted(os.environ.items()): 
    print(item)
