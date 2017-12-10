using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class CSVReader : MonoBehaviour 
{
    public TextAsset fileToRead;

    public static string[] ReadCSV(TextAsset file, string sep = "\n") 
    {
        string[] lines = file.text.Split(sep[0]);
        for (int i = 0; i < lines.Length; i++) 
        {
           // Debug.Log(i + " " + lines[i]);
        }

        return lines;
    }

    public static string[] ReadLine(TextAsset file, int line, string sep = "\n")
    {
       // List<string> output = new List<string>();
        string[] lines = file.text.Split(sep[0]);
        
        string[] rows = lines[line].Split(","[0]);

        return rows;

    }

    public static int GetLength(TextAsset file, string sep = "\n", bool ignore = true)
    {

        string[] lines = file.text.Split(sep[0]);

        int l = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (ignore && i == 0 || lines[i] == "") { }

            else
            { 
                l++;
                //Debug.Log(i + " " + lines[i]);
            }
        }
        return l;

    }


    void Start() 
    {
        /*
        string[] test = ReadLine(fileToRead, 5);
        Debug.Log("line " + test);
        Debug.Log("str " + test[2]);
        */
    }
    
}
