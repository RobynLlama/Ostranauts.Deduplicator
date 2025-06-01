using System.Collections.Generic;
using System.Text.RegularExpressions;
using Deduplicator;

public static partial class Callbacks
{
  /// <summary>
  /// Once we hear that a game has finished loading we should
  /// be able to check if the player has duplicate conditions
  /// and maybe do something about that
  /// </summary>
  public static void OnGameFinishedLoadingCallback()
  {

    var player = CrewSim.coPlayer;
    List<string> condNames = [];
    List<string> duplicates = [];

    bool IsDuplicateConditionInList(string condName)
    {
      foreach (var item in condNames)
        if (AreDuplicateConditionNames(condName, item))
          return true;
      return false;
    }

    foreach (Condition cond in player.mapConds.Values)
    {
      if (IsDuplicateConditionInList(cond.strName))
        duplicates.Add(cond.strName);
      condNames.Add(cond.strName);
    }



    //Delete dupes
    foreach (string cond in duplicates)
    {
      Plugin.Log.LogMessage($"Removing duplicate condition {cond} by setting to -1");
      CrewSim.coPlayer.LogMessage($"Duplicate condition {cond} removed", "Neutral", "DCD-Plugin");
      player.AddCondAmount(cond, -1);
    }


  }

  /// <summary>
  /// A list of conditions that are actually allowed to be duplicates
  /// and otherwise meet the conditions to be removed
  /// </summary>
  private static readonly List<string> whiteList = [
    "DcAging01",
    "DcAging02",
    "DcAging03",
    "DcAging04",
    "DcAging05"
  ];

  /// <summary>
  /// Precompiled Regex for enforcing condition name conformity
  /// This prevents bad things, like the duplicate list finding
  /// various quest flags that end in digits as duplicates
  /// </summary>
  private static readonly Regex regex = new(@"^Dc.*\d{2}$", RegexOptions.Compiled);

  /// <summary>
  /// Determines whether two condition names are considered equivalent by ignoring 
  /// certain appended information, such as magnitude, that may interfere with direct string comparisons.
  /// </summary>
  /// <param name="condA">The first condition name, which should be taken directly from a condition object.</param>
  /// <param name="condB">The second condition name, which should be taken directly from a condition object.</param>
  /// <returns>
  /// A boolean value indicating whether the specified condition names represent 
  /// the same underlying condition. Returns <c>true</c> if the conditions are considered equivalent; otherwise, <c>false</c>.
  /// </returns>
  /// <remarks>
  /// The comparison accounts for magnitudes appended to condition names (e.g., DcFatigue01 and DcFatigue02).
  /// Additionally, it considers that conditions can store quest data sequentially, meaning that while they 
  /// share the same base name without magnitude, they may actually represent different conditions.
  /// </remarks>
  internal static bool AreDuplicateConditionNames(string condA, string condB)
  {
    if (condA is null || condB is null)
    {
      Plugin.Log.LogError("Encountered a null condition, the save may be corrupted!");
      return false;
    }

    //if the strings are exactly equal
    if (condA.Equals(condB))
      return true;

    //if either string is on the whitelist
    if (whiteList.Contains(condA) || whiteList.Contains(condB))
      return false;

    /*
      if the strings both conform to the following:
        1. Start with `Dc`
        2. End with 2 digits
    */

    if (!regex.IsMatch(condA) || !regex.IsMatch(condB))
      return false;

    //only if the strings are big enough for substring
    if (condA.Length > 2 && condB.Length > 2)
      return condA.Substring(0, condA.Length - 2) == condB.Substring(0, condB.Length - 2);

    //just return false if they aren't duplicates and can't be trimmed
    return false;
  }
}
