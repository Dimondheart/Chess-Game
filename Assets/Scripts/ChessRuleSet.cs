using UnityEngine;
using System.Collections;

/** Contains all parameters for controlling the rules of a chess game. */
[System.Serializable]
public sealed class ChessRuleSet
{
  public enum TimeControlMode
  {
    NONE = 0
  }

  /** The name of this rule set. */
  public string ruleSetName;
  /** If a pawn is allowed to move two spaces the first time they move. */
  public bool pawnCanMoveTwoOnFirstMove = true;
  public bool castlingEnabled = true;
  public bool enPassantEnabled = true;
  public bool promotionEnabled = true;
  /** If true, for a player to promote to a certain piece, at least one of their pieces of that color must first
   * be captured.
   */
  public bool promotionOnlyToCaptured = false;
  /** If piece promotions are allowed to exceed the number of pieces of each type for their color initially
   * on the board.
   */
  public bool promotionLimitTypeCount = false;
  /** If a player is allowed to take an action that causes stalemate for the other player. */
  public bool stalemateAllowed = true;
  public TimeControlMode timeControlMode = TimeControlMode.NONE;
  /** If true, when a player runs out of time they instantly lose. Otherwise the current player ends their turn. */
  public bool timeoutCausesSuddenDeath = false;

  public ChessRuleSet() : this("New Rule Set")
  {
  }

  public ChessRuleSet(string name)
  {
    ruleSetName = name;
  }
}
