using UnityEngine;

class Canon {
    public Vector3 power = Vector3.zero;
    public int id;
    
    public Canon(int canonId) {
        this.id = canonId;
    }

    public void setPower(Vector3 power) {
        this.power = power;
    }

    public void addPower(Vector3 power) {
        this.power += power;
    }
}