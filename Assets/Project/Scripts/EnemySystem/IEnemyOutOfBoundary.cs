using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyOutOfBoundary
{
    IEnumerator GroundCheckerCoroutine();
    void GroundChecker();
    void ResumeChecker();
}
