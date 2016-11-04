using UnityEngine;
using System.Collections;

[System.Serializable]
public class ChessRuleSet
{
  public enum TimeControlMode
  {
    NONE = 0
  }

  /** The name of this rule set. */
  public string ruleSetName;
  public bool castlingEnabled = true;
  public bool enPassantEnabled = true;
  public bool promotionEnabled = true;
  public bool promotionOnlyToCaptured = false;
  public bool promotionLimitTypeCount = false;
  public bool stalemateEnabled = true;
  public bool pawnCanMoveTwoOnFirstMove = true;
  public TimeControlMode timeControlMode = TimeControlMode.NONE;
  public bool timeoutCausesSuddenDeath = false;

  public ChessRuleSet() : this("New Rule Set")
  {
  }

  public ChessRuleSet(string name)
  {
    ruleSetName = name;
  }
}
