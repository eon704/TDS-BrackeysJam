using UnityEngine;


public static class ScreenUtilities {
  public static Vector3 ScreenPointToWorldPoint(this Vector3 vector, float cameraOffset) {
    return Camera.main
      ? Camera.main.ScreenToWorldPoint(vector + new Vector3(0f, 0f, cameraOffset))
      : vector;
  }
}
