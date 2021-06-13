using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;




public class LSystemTree : MonoBehaviour
{
    public int iterations = 4;    
    public float length = 10f;        
    public float angle = 30f;
    private const string w0 = "X";          //initial value w(n) = X, for n =0

    public GameObject Branch;

    private string currentIteration = string.Empty;
    private string previousIteration = string.Empty;

    private Stack<TransformValues> transformValues;     //stores currentIterations in stack
    private Dictionary<char, string> rules;

    void Start()
    {
        transformValues = new Stack<TransformValues>();

        DefineRules();

        StartCoroutine(Waiter());
    }


    private void DefineRules()
    {
        rules = new Dictionary<char, string>
        {
            {'X', "F+[[X]-X]-F[-FX]+X"},      //rule for X
            {'F', "FF" }                      //rule for F
        };
    }

    IEnumerator Waiter()
    {
        currentIteration = w0;

        for (int i = 0; i < iterations; i++)
        {
           
            currentIteration = GetCurrentIterationString();
            GenerateTree(currentIteration);

            ResetStartValuesForNewIteration();

            yield return new WaitForSeconds(0.5f);

        }
    }

    private string GetCurrentIterationString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in currentIteration)
        {
            sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());    //if character from rules is in string (as char c) then replace it with string from rules
        }

        return sb.ToString();

    }

    public void GenerateTree(string currentIteration)
    {
        //iterate through current Iteration String
        foreach (char c in currentIteration)
        {           
            switch (c)
            {
                case 'F':
                    DrawStraightLine();
                    break;
                case 'X':   //do nothing - a symbol to generate FF
                    break;
                case '-':
                    MoveLeft();
                    break;
                case '+':  
                    MoveRight();
                    break;
                case '[':  
                    SaveCurrentTransformValue();
                    break;
                case ']':  
                    GetPreviousTransformValue();
                    break;
                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
        }
    }

    private void DrawStraightLine()
    {
        Vector3 initialPosition = transform.position;
        transform.Translate(Vector3.up * length);
        GameObject treeSegment = Instantiate(Branch);
        treeSegment.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
        treeSegment.GetComponent<LineRenderer>().SetPosition(1, transform.position);
    }

    private void MoveLeft()
    {
        transform.Rotate(Vector3.forward * angle);
    }
    private void MoveRight()
    {
        transform.Rotate(Vector3.back * angle);
    }

    private void SaveCurrentTransformValue()
    {
        transformValues.Push(new TransformValues()
        {
            postition = transform.position,
            rotation = transform.rotation
        });
    }
    private void GetPreviousTransformValue()
    {
        TransformValues ti = transformValues.Pop();
        transform.position = ti.postition;
        transform.rotation = ti.rotation;
    }

    private void ResetStartValuesForNewIteration()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }



}
