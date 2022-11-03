using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excercises : MonoBehaviour
{
    private int[] numbers;
    
    // Start is called before the first frame update
    void Start()
    {

        
        Excercise5();
        
    }

    void Excercise5()
    {
        numbers = new[] {6, 2, 5, 6, 7, 3, 2, 4, 5, 6};

        for (int i = 0; i < numbers.Length ; i++)
        {
            print(numbers[i]);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
