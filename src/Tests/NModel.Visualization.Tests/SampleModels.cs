using NModel;
using NModel.Attributes;
using NModel.Execution;
using System.Collections.Generic;

namespace SampleModels.PowerSwitch
{
    public enum Power { On, Off };
    public static class Contract
    {
        static Power power = Power.Off;

        [Action]
        static void PowerOn() { power = Power.On; }
        static bool PowerOnEnabled() { return power == Power.Off; }

        [Action]
        static void PowerOff() { power = Power.Off; }
        static bool PowerOffEnabled() { return power == Power.On; }
    }
}

namespace SampleModels.Fan
{
    public enum CONTROL { Power, Speed };
    [Feature]
    public static class Control
    {
        public static CONTROL focus = CONTROL.Power;

        [Action]
        static void ControlPower() { focus = CONTROL.Power; }
        static bool ControlPowerEnabled() { return focus == CONTROL.Speed; }

        [Action]
        static void ControlSpeed() { focus = CONTROL.Speed; }
        static bool ControlSpeedEnabled() { return focus == CONTROL.Power; }

        [Action("PowerOn")]
        [Action("PowerOff")]
        static void PowerLoop() { }
        static bool PowerLoopEnabled() { return focus == CONTROL.Power; }

        [Action("IncrementSpeed")]
        static void SpeedLoop() { }
        static bool SpeedLoopEnabled() { return focus == CONTROL.Speed; }
    }

    public enum POWER { On, Off };
    [Feature]
    public static class Power
    {
        public static POWER power = POWER.Off;

        [Action]
        static void PowerOn() { power = POWER.On; }
        static bool PowerOnEnabled() { return power == POWER.Off; }

        [Action]
        static void PowerOff() { power = POWER.Off; }
        static bool PowerOffEnabled() { return power == POWER.On; }
    }

    public class SPEED : CompoundValue
    {
        string val;

        public SPEED(string val) { this.val = val; }

        public static SPEED Low = new SPEED("Low");
        public static SPEED Medium = new SPEED("Medium");
        public static SPEED High = new SPEED("High");
    }

    [Feature]
    public static class Speed
    {
        public static SPEED speed = SPEED.Low;

        [Action]
        static void IncrementSpeed()
        {
            speed = (speed == SPEED.Low ? SPEED.Medium : (speed == SPEED.Medium ? SPEED.High : SPEED.Low));
        }
    }

    [Feature,Domain("new")]
    public class Filter1 : LabeledInstance<Filter1>
    {
        static IEnumerable<SPEED> P { get { yield return SPEED.High; } }

        static IEnumerable<Filter1> THIS { get { yield return Filter1.Create(); } }

        [Action]
        void act([Domain("P")]CompoundValue i /* Sequence<int> x */)
        {
        }

        [StateFilter]
        public static bool Filter()
        {
            return !(Control.focus == CONTROL.Speed &&
                   Power.power == POWER.On &&
                   Speed.speed == SPEED.High);
        }
    }
}


