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

NumNewData = 3

dp = "C:\\Users\\yzt8562\\Proj\\Assets\\covtype_w_outlier.csv"
df = pd.read_csv(dp).head(100)

while True:
  if start == 0:
    message = socket.recv(NumNewData*4)
    received = np.frombuffer(message,dtype =np.float32).reshape(1,NumNewData)[0]
    print(received)
    start = 1

    # dataframe delete Outlier update
    X = df[["Elevation", "Slope","Cover_Type"]]
    # X['Elevation'] = X['Elevation'] /1000
    estimator = LocalOutlierFactor(n_neighbors=14)
    estimated_result = estimator.fit_predict(X)

    outlier_ind = []
    x = 0
    for i in range (len(estimated_result)):
      if estimated_result[i] <0:
        outlier_ind.append(i)
        df = df.drop([df.index[i-x]])
        x+=1
    outlier = np.array(outlier_ind)
    print(outlier_ind)
    outlier_byte = outlier.tobytes()
    socket.send(outlier_byte)

  else:
    # update 5 each time
    message = socket.recv(NumNewData*4) # the new one line of data -- year experience and salary
    array_received = np.frombuffer(message,dtype=np.float32).reshape(1,NumNewData)[0]
    df = df.head(99)
    df.loc[len(df.index)] = array_received
    print(array_received)
    estimator = LocalOutlierFactor(n_neighbors=14)
    estimated_result = estimator.fit_predict(df)
    outlier_ind = []
    x = 0
    for i in range (len(estimated_result)):
      if estimated_result[i] <0:
        outlier_ind.append(i)
        df = df.drop([df.index[i-x]])
        x+=1
    outlier = np.array(outlier_ind)
    print(outlier_ind)
    outlier_byte = outlier.tobytes()
    socket.send(outlier_byte)