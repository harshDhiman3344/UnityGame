// Decompiled with JetBrains decompiler
// Type: PlayerInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 949EE0B5-C93E-4EEF-B425-9C9A3B075F73
// Assembly location: D:\Program Files\JellyDrift64\Jelly Drift_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PlayerInput : MonoBehaviour
{
  public Car car;

  private void GetPlayerInput()
  {
    this.car.steering = Input.GetAxisRaw("Horizontal");
    this.car.throttle = Input.GetAxis("Vertical");
    this.car.breaking = Input.GetButton("Breaking");
  }

  private void Update()
  {
    // if ((bool) (Object) GameController.Instance && !GameController.Instance.playing)
    //   return;
    this.GetPlayerInput();
  }
}
