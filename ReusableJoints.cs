namespace HandyUtilities
{
    using UnityEngine;

    public class ReusableJoint
    {
        public Joint joint
        {
            get
            {
                Restore();
                return m_hinge;
            }
        }

        Vector3 m_conAnchor, m_anchor, m_axis;
        Joint m_hinge;
        Rigidbody m_connectedBody;
        JointLimits m_limits;
        JointMotor m_motor;
        GameObject m_target;

        public ReusableJoint(Joint hinge)
        {
            this.m_hinge = hinge;
            this.m_connectedBody = hinge.connectedBody;
            this.m_anchor = hinge.anchor;
            this.m_conAnchor = hinge.connectedAnchor;
            this.m_axis = hinge.axis;
            this.m_target = hinge.gameObject;
        }

        public virtual void Break()
        {
            Object.Destroy(joint);
        }

        public virtual bool Restore()
        {
            if (m_hinge == null)
            {
                m_hinge = m_target.AddComponent<HingeJoint>();
                m_hinge.connectedBody = m_connectedBody;
                m_hinge.anchor = m_anchor;
                m_hinge.connectedAnchor = m_conAnchor;
                m_hinge.axis = m_axis;
                return true;
            }
            return false;
        }
    }

    public class ReusableHingeJoint : ReusableJoint
    {
        JointMotor m_motor;
        JointLimits m_limits;

        public ReusableHingeJoint(HingeJoint hinge) : base(hinge)
        {
            this.m_limits = hinge.limits;
            this.m_motor = hinge.motor;
        }

        public override bool Restore()
        {
            if (base.Restore())
            {
                var h = (HingeJoint) joint;
                h.limits = m_limits;
                h.motor = m_motor;
                return true;
            }
            return false;
        }
    }
}