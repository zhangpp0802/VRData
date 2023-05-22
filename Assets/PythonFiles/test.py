## WEB SOCKET SERVER
# process the data with python packages.
import zmq
import sklearn
from sklearn.neighbors import LocalOutlierFactor
import numpy as np
import pandas as pd

# web socket setup
start = 0
ctx = zmq.Context()
socket = ctx.socket(zmq.REP)
socket.bind("tcp://127.0.0.1:5555")

while True:
  if start == 0:
    message = socket.recv(8)
    received = np.frombuffer(message,dtype =np.float32).reshape(1,2)[0]
    start = 1

    # dataframe delete Outlier update
    dp = "C:\\Users\\yzt8562\\Proj\\Assets\\Salary_dataset.csv"
    df = pd.read_csv(dp)
    X = df[["YearsExperience", "Salary"]]
    X['Salary'] = X['Salary'] /10000
    estimator = LocalOutlierFactor(n_neighbors=6)
    estimated_result = estimator.fit_predict(X)

    outlier_ind = []
    for i in range (len(estimated_result)):
      if estimated_result[i] <0:
        outlier_ind.append(i)
        df = df.drop([df.index[i]])
    
    socket.send(bytes(outlier_ind))

  else:
    message = socket.recv(8) # the new one line of data -- year experience and salary
    array_received = np.frombuffer(message,dtype=np.float32).reshape(1,2)[0]
    print(array_received)
    yearExp = array_received[0]
    salary = array_received[1]

    print(yearExp,salary) # yearsexperience:2; salary:3