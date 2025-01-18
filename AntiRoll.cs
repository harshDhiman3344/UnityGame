// Decompiled with JetBrains decompiler
// Type: AntiRoll
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 949EE0B5-C93E-4EEF-B425-9C9A3B075F73
// Assembly location: D:\Program Files\JellyDrift64\Jelly Drift_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class AntiRoll : MonoBehaviour
{
  public Suspension right;
  public Suspension left;
  public float antiRoll = 5000f;
  private Rigidbody bodyRb;

  private void Awake() => this.bodyRb = this.GetComponent<Rigidbody>();

  private void FixedUpdate() => this.StabilizerBars();

  private void StabilizerBars()
  {
    float num1 = !this.right.grounded ? 1f : this.right.lastCompression;
    float num2 = ((!this.left.grounded ? 1f : this.left.lastCompression) - num1) * this.antiRoll;
    if (this.right.grounded)
      this.bodyRb.AddForceAtPosition(this.right.transform.up * -num2, this.right.transform.position);
    if (!this.left.grounded)
      return;
    this.bodyRb.AddForceAtPosition(this.left.transform.up * num2, this.left.transform.position);
  }
}
