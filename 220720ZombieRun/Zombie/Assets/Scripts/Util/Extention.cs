using UnityEngine;

public static class Extension
{
    // 정적 클래스 타입, 이름은 보통 Extension으로 붙이는 편
    // 퍼블릭 정적 메소드로 만들어야 함
    // 접근한정자 정적 반환타입 이름(this포인터가 들어갈 자리, ...)
    public static void SetIKPositionAndWeight(this Animator animator, AvatarIKGoal goal, Vector3 goalPosition, float weight = 1f)
    {
        animator.SetIKPositionWeight(goal, weight);
        animator.SetIKPosition(goal, goalPosition);
    }

    public static void SetIKRotationAndWeight(this Animator animator, AvatarIKGoal goal, Quaternion goalRotation, float weight = 1f)
    {
        animator.SetIKRotationWeight(goal, weight);
        animator.SetIKRotation(goal, goalRotation);
    }
    public static void SetIKTransformAndWeight(this Animator animator, AvatarIKGoal goal, Transform goalTransform, float weight = 1f)
    {
        SetIKPositionAndWeight(animator, goal, goalTransform.position, weight);
        SetIKRotationAndWeight(animator, goal, goalTransform.rotation, weight);
    }
}