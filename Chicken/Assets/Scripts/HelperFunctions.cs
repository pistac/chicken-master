using System.Collections;
using System.Collections.Generic;
using System;

// Class containing static helper functions that are accessible from everywhere
// but fit nowhere else in the code.
public static class HelperFunctions {
  private static Random rng = new Random();

  // Random list shuffle method proposed by stackoverflow user grenade:
  // https://stackoverflow.com/questions/273313/randomize-a-listt
  public static void Shuffle<T>(this IList<T> list) {
    int n = list.Count;
    while (n > 1) {
      n--;
      int k = rng.Next(n + 1);
      T value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
  }
}
