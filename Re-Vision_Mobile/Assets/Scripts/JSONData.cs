using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class JSONData
{
    public string id;
    public string theme;
    public string name;
    public string url;
    public List<QuestionsList> questions;
    public List<RewardsList> rewards;
}

[Serializable]
public class QuestionsList
{
    public int id;
    public string question;
    public string image;
    public int correct;
    public string answers1;
    public string answers2;
    public string answers3;
    public string answers4;

}

[Serializable]
public class RewardsList
{
    public int id;
    public string type;
    public string image;
    public string criteria;
    public int criteria_quantity;

}
