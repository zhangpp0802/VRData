using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;
using UnityEditor.Scripting.Python;


public class Datapoints : MonoBehaviour
{
    public GameObject[] datalist;
    // Start is called before the first frame update
    void Start()
    {
        datalist = new GameObject[31];
        ReadCSV();
    }

    void ReadCSV(){
        StreamReader strreader = new StreamReader("C:\\Users\\yzt8562\\Proj\\Assets\\Salary_dataset.csv");
        bool end = false;
        bool start = true;
        var i = 0;
        while(!end){
            string data = strreader.ReadLine();
            if(data == null){
                end = true;
                break;
            }
            if (!start){
                var data_value = data.Split(",");
                var year = float.Parse(data_value[1]);
                var salary = float.Parse(data_value[2])/10000;
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(year, salary, 0);
                cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                datalist[i] =cube;
                i++;

                Rigidbody cubeRig = cube.AddComponent<Rigidbody>();
                cubeRig.useGravity = false;
                XRGrabInteractable xrgrab = cube.AddComponent<XRGrabInteractable>();
                xrgrab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
                xrgrab.smoothPosition = true;
                xrgrab.forceGravityOnDetach = true;
            }
            else{
                start = false;
            }
        }
    }

    // Update is called once per frame 1/72
    void Update()
    {
        for (int i = 0; i < datalist.Length; i++){
            GameObject cube = datalist[i];
            Rigidbody cubeRig = cube.GetComponent<Rigidbody>();
            XRGrabInteractable xrgrab = cube.GetComponent<XRGrabInteractable>();
            if (cubeRig.useGravity){
                // eleminateItem();
                cube.transform.position = new Vector3(0, 0, 0);
            } 
            
        }
        // FileRunnerFromPython();
    }
    void StringRunnerFromPython()
    {
        PythonRunner.RunString(@"
                import UnityEngine;
                UnityEngine.Debug.Log('hello world')
                ");
    }

    void FileRunnerFromPython()
    {
        // string dp = "Assets/PythonFiles/test.py";
        string dp = "Assets/PythonFiles/test.py";
        PythonRunner.RunFile(dp);
    }
}
