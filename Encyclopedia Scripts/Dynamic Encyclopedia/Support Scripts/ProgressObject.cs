using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ProgressObject : MonoBehaviour {

    public ProgressPaths ProgressPaths;
    private static ProgressObject _instance;

    public static ProgressObject Instance { get { return _instance; } }


	void Awake () {

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        BinaryFormatter bf = new BinaryFormatter();

        if (!File.Exists(Application.persistentDataPath + "/progress.dat"))
        {
            FileStream file = File.Create(Application.persistentDataPath + "/progress.dat");

            ProgressPaths newPaths = new ProgressPaths();
            newPaths.progressPaths = new List<ProgressPath>();
            newPaths.progressPaths.Add(new ProgressPath(1, 1));
            newPaths.progressPaths.Add(new ProgressPath(1, 2));
            bf.Serialize(file, newPaths);
            file.Close();
        }

        FileStream openFile = File.Open(Application.persistentDataPath + "/progress.dat", FileMode.Open);
        this.ProgressPaths = (ProgressPaths)bf.Deserialize(openFile);
        openFile.Close();
    }
}
