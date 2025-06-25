using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class ValidationScript : MonoBehaviour
{
    //KTANE Variables
    public KMBombInfo _Bomb;
    public KMBossModule Boss;
    public KMNeedyModule Needy;
    public KMSelectable Dial;

    //Unity Variables
    public Material[] OffOn;
    public Animator Needle;
    public Renderer[] LEDs;

    //Script Variables
    private int SelectedNumber;
    private bool ValidState;
    private int[] ledStates = new int[] { 0, 1 };
    private int _moduleId;
    static int _moduleIdCounter = 1;
    private bool NeedyActive;
    private string lastInput;
    private string[] validLetters;

    //Thank You Blananas2
    private int Solves;
    private string MostRecent;
    private List<string> SolveList = new List<string> { };
    private string[] IgnoredModules;

    void Awake()
    {
        _moduleId = _moduleIdCounter++;
        GetComponent<KMNeedyModule>().OnNeedyActivation += NeedyStart;
        //GetComponent<KMNeedyModule>().OnNeedyDeactivation += YouFuckedUp;
        Dial.OnInteract += delegate { DialToggle(); return false; };

        if (IgnoredModules == null) {
            IgnoredModules = Boss.GetIgnoredModules(Needy, new string[]{ //Potentially excessive, but anything with 'may solve with others' is safer
                "The Twin", "X", "Y", "Castor", "Pollux", "Apple Pen", "Pineapple Pen", "Reporting Anomalies", "Solve Shift", "Tyler Verifies"
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Please Fuck me");

        if (NeedyActive)
        {
            //This keeps the timer number set to the selected number
            if (Needy.GetNeedyTimeRemaining().ToString() != SelectedNumber.ToString())
            {
                Needy.SetNeedyTimeRemaining(SelectedNumber);
            }
        }
        //This calls the method to evaluate if the last solve was correct
        if (Solves != _Bomb.GetSolvedModuleNames().Count())
        {
            MostRecent = GetLatestSolve(_Bomb.GetSolvedModuleNames(), SolveList);
            SolveList.Add(MostRecent);
            MostRecent = SolveList[Solves];
            Solves++; //thank you exish for this clever solution --blan

            if (IgnoredModules.Contains(MostRecent))
            {
                Debug.Log("Ignored module " + MostRecent + " has been solved.");
                return;
            }

            if (NeedyActive)
            {
                LastSolveEvaluation();
            }
        }
    }

    void NeedyStart()
    {
        NeedyActive = true;
        Debug.Log("Module Started");

        //For testing Selected Cases
        //SelectedNumber = 2;
        SelectedNumber = Rnd.Range(1, 6);
        for (int i = 0; i < ledStates.Length; i++)
            ledStates[i] = Rnd.Range(0, 2);

        for (int i = 0; i < LEDs.Length; i++)
            LEDs[i].material = OffOn[ledStates[i]];

        Debug.LogFormat("[Validation #{0}] LED States from left to right are {1} and {2}.", _moduleId, ledStates[0] == 1 ? "on" : "off", ledStates[1] == 1 ? "on" : "off");
        EvaluateValidLetters();
    }

    void EvaluateValidLetters()
    {
        switch (SelectedNumber)
        {
            case 1:
                switch (ledStates[0])
                {
                    case 0:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "r", "e", "b" };
                                Debug.Log("Number 1, Off, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "h", "y", "o", "a", "f", "t", "e", "r", "w" };
                                Debug.Log("Number 1, Off, On");
                                break;
                        }
                        break;
                    case 1:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "l", "s", "a", "t", "n", "m", "o", "b" };
                                Debug.Log("Number 1, On, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "e", "o", "p", "w", "a", "r", "t", "l", "d" };
                                Debug.Log("Number 1, On, On");
                                break;
                        }
                        break;
                }
                break;
            case 2:
                switch (ledStates[0])
                {
                    case 0:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "y", "m", "r", "u", "d", "a", "e", "f", "l" };
                                Debug.Log("Number 2, Off, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "i", "a", "m", "o", "n", "p", "w" };
                                Debug.Log("Number 2, Off, On");
                                break;
                        }
                        break;
                    case 1:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "g", "a", "w", "v", "e", "r", "i", "n", "f" };
                                Debug.Log("Number 2, On, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "l", "g", "n", "r", "f", "d", "i", "e", "a" };
                                Debug.Log("Number 2, On, On");
                                break;
                        }
                        break;
                }
                break;
            case 3:
                switch (ledStates[0])
                {
                    case 0:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "d", "l", "j", "g", "i", "a", "b", "n", "e" };
                                Debug.Log("Number 3, Off, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "o", "a", "f", "e", "r", "d" };
                                Debug.Log("Number 3, Off, On");
                                break;
                        }
                        break;
                    case 1:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "u", "l", "y", "a", "r", "e", "b", "f", "i" };
                                Debug.Log("Number 3, On, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "h", "y", "a", "e", "d", "l" };
                                Debug.Log("Number 3, On, On");
                                break;
                        }
                        break;
                }
                break;
            case 4:
                switch (ledStates[0])
                {
                    case 0:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "u", "a", "r", "g", "h", "s" };
                                Debug.Log("Number 4, Off, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "n", "a", "u", "m", "e", "t", "l" };
                                Debug.Log("Number 4, Off, On");
                                break;
                        }
                        break;
                    case 1:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "l", "r", "g", "z", "i", "p", "e", "t", "m", "y", "l" };
                                Debug.Log("Number 4, On, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "t", "n", "e", "s", "r", "o", "x", "b", "c", "a" };
                                Debug.Log("Number 4, On, On");
                                break;
                        }
                        break;
                }
                break;
            case 5:
                switch (ledStates[0])
                {
                    case 0:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "n", "m", "o", "p", "i", "a" };
                                Debug.Log("Number 5, Off, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "i", "a", "h", "n", "s", "b", "t", "e" };
                                Debug.Log("Number 5, Off, On");
                                break;
                        }
                        break;
                    case 1:
                        switch (ledStates[1])
                        {
                            case 0:
                                validLetters = new string[] { "o", "t", "l", "h", "b", "g", "i" };
                                Debug.Log("Number 5, On, Off");
                                break;
                            case 1:
                                validLetters = new string[] { "i", "r", "m", "k", "e", "t", "a", "o", "n" };
                                Debug.Log("Number 5, On, On");
                                break;
                        }
                        break;
                }
                break;
        }
        Debug.LogFormat("[Validation #{0}] Valid Letters are: {1}", _moduleId, string.Join(", ", validLetters));
    }

    void DialToggle()
    {
        if (ValidState == false)
        {
            ValidState = true;
            Needle.SetTrigger("Valid");
        }
        else
        {
            ValidState = false;
            Needle.ResetTrigger("Valid");
        }
    }

    void LastSolveEvaluation()
    {
        var module = MostRecent;
        if (module.StartsWith("The "))
        {
            module = module.Substring(4);
            Debug.Log("The word to evaluate after removing 'The' is " + module);
        }
        else
            Debug.Log("The word to evaluate is " + module);
        lastInput = module.Substring(0, 1).ToLower();
        Debug.Log("You have input " + lastInput);

        //This half evaluates if the input is truly valid. This switch makes me want to shoot my computer.
        Debug.Log(validLetters.Contains(lastInput) ? "Valid Input Recieved" : "Invalid Input Recieved");
        if (ValidState == validLetters.Contains(lastInput))
        {
            Debug.Log("Needle position matches required input state");
            return;
        }
        else
        {
            for (int i = 0; i < ledStates.Length; i++)
                LEDs[i].material = OffOn[0];
            Debug.Log("Needle position does not match required input state");
            YouFuckedUp();
        }
    }

    void YouFuckedUp()
    {
        if (_Bomb.GetSolvedModuleNames().Count() < _Bomb.GetSolvableModuleNames().Count())
        {
            Needy.HandleStrike();
            Debug.Log("[Validation #" + _moduleId + "] Strike Issued");
            Needy.HandlePass();
            NeedyActive = false;
        }
        else
        {
            Debug.LogFormat("[Validation #{0}] Validation state for the last the module was wrong, ya noodle.",_moduleId);
            Needy.HandlePass();//Have a Gold Star for trying though.
            NeedyActive = false;
        }
    }

    private string GetLatestSolve(List<string> a, List<string> b)
    {
        string z = "";
        for (int i = 0; i < b.Count; i++)
        {
            a.Remove(b.ElementAt(i));
        }

        z = a.ElementAt(0);
        return z;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} tap [Taps the meter]";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*tap\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            Dial.OnInteract();
        }
    }
}
