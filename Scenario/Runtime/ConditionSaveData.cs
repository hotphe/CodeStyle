using System.Collections.Generic;
using UnityEngine;
using PCS.SaveData;
using System.Linq;

public class ConditionSaveData : SaveData<ConditionSaveData>
{
    [SerializeField] private List<ConditionInfo> _conditions = new List<ConditionInfo>();

    public void AddCondition(ConditionInfo condition)
    {
        if (IsConditionExist(condition))
            return;
        _conditions.Add(condition);
    }

    private bool IsConditionExist(ConditionInfo condition)
    {
        int conditionId = condition.ConditionID;
        int type = condition.Type;

        var list = _conditions.Where(x => x.ConditionID == conditionId && x.Type == type).ToList();
        if (list.Count > 0)
            return true;
        return false;
    }

    public bool ContainCondition(int conditionId)
    {
        var list = _conditions.Where(x=>x.ConditionID == conditionId).ToList();
        if(list.Count > 0) 
            return true;
        return false;
    }

    public List<ConditionInfo> GetAllConditions() { return _conditions; }
}