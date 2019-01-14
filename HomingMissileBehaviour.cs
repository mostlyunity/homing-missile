public class HomingMissileBehaviour {

    [SerializeField] private float _angularSpeed = 8;
    [SerializeField] private float _speed = 20;

    private Transform _target;
    private Vector3 _waypoint;
    private bool _passedWaypoint;

    public void Update() {
        if (_target == null) {
            return;
        }

        Vector3 currentGoal;

        if (!_passedWaypoint && _waypoint != default) {
            currentGoal = _waypoint;

            if (Vector3.Distance(transform.position, _waypoint) < 2) {
                _passedWaypoint = true;
                _waypoint = default;
            }
        } else {
            var targetCenter = _target.GetGeometricalCenter();

            currentGoal = targetCenter;
        }

        var direction = (currentGoal - transform.position).normalized;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            _angularSpeed * Time.deltaTime);
        transform.Translate(0, 0, _speed * Time.deltaTime);
    }

    public void FixedUpdate() {
        if (_target == null) {
            return;
        }

        const int CheckRange = 10;
        var isHit = Physics.Raycast(transform.position, transform.forward, out var hitInfo, CheckRange);

        if (isHit && hitInfo.collider.transform == _target || !isHit) {
            return;
        }

        var tiltFactor = transform.lossyScale.x;
        var sign = hitInfo.point.x > hitInfo.collider.transform.position.x ? 1 : -1;
        var xBound = sign > 0 ? 
            hitInfo.collider.transform.position.x + hitInfo.collider.transform.lossyScale.x / 2 + tiltFactor :
            hitInfo.collider.transform.position.x - hitInfo.collider.transform.lossyScale.x / 2 - tiltFactor;
        var waypoint = new Vector3(xBound, hitInfo.point.y, hitInfo.point.z);

        _passedWaypoint = false;
        _waypoint = waypoint;
    }

    public void SetTarget(Transform target) {
        _target = target;
    }

}