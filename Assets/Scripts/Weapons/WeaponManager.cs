using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]CharacterBase character;

    [Header("Weapon Settings")]
    [SerializeField] private float weaponRadius = 1f; // 무기들이 배치될 원의 반지름

    public WeaponBase Weapon { get; private set; }
    private List<WeaponBase> weapons = new List<WeaponBase>();

    void Start()
    {
        ArrangeExistingWeapons();
    }

    // 디버깅용: 기존 무기들을 받아서 배치
    public void ArrangeExistingWeapons()
    {
        // 자식 오브젝트에서 모든 Weapon 컴포넌트를 찾습니다
        WeaponBase[] existingWeapons = GetComponentsInChildren<WeaponBase>();
        
        // 기존 리스트 초기화
        weapons.Clear();
        
        // 찾은 무기들을 리스트에 추가
        weapons.AddRange(existingWeapons);
        
        // 무기들 배치
        SetWeaponPositions(weapons.Count, weaponRadius);
        
        Debug.Log($"기존 무기 {weapons.Count}개를 배치했습니다.");
    }

    public void AssignWeapon(WeaponBase weapon, int weaponLevel)
    {
        WeaponBase newWeapon = Instantiate(weapon, transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        
        weapons.Add(newWeapon);
        SetWeaponPositions(weapons.Count, weaponRadius);
    }

    public void RemoveWeapon()
    {
        if (Weapon != null)
        {
            weapons.Remove(Weapon);
            Destroy(Weapon.gameObject);
            Weapon = null;
            SetWeaponPositions(weapons.Count, weaponRadius);
        }
    }

    private void SetWeaponPositions(int weaponCount, float radius, float angleOffset = 0)
    {
        if (weaponCount <= 0) return;

        float angleStep = 360f / weaponCount;
        for (int i = 0; i < weaponCount; i++)
        {
            float angle = (angleOffset + i * angleStep) * Mathf.Deg2Rad;
            Vector3 offset = character.CenterPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            weapons[i].transform.localPosition = offset;
            
            // 무기가 바깥쪽을 향하도록 회전
            float rotationAngle = i * angleStep;
            weapons[i].transform.localRotation = Quaternion.Euler(0, 0, rotationAngle);
        }
    }
}
