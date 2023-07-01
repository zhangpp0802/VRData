# VRData
* Author: Yiran Zhang

This is an on-going research with Dr. Diego Klabjan at Northwestern University. You can clone and download and access the scene for VR Oculus. 


To Run:
* You need to go to the PythonFiles folder in Assets and run the test.py.
* Then, while the Python file running, you can run the main Unity scene in Unity, and in your Oculus, you will see the scene.


Current Stage:
* Now, the sklearn outlier detection is implemented.
* This scene also allows manually select the data point you want to delete.
* Larger 3D dataset http://archive.ics.uci.edu/ml/datasets/covertype
* Dynamic plot

Working on:
* Update too fast
* Scale the variables (in the same viewport)
* dynamic data mapping dynamic threshold


Note:
* This program is large. If you only want to see the sample dataset (salary dataset) plot with the manually selecting outlier feature, you can connect the Datapoints object in the scene to the Datapoint script instead of Datapoint C script. 
* Ignore the error message in Unity Console for now, Yiran is still fixing the code.
