﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Threading;

namespace UTFEditor
{
    public partial class FuseEditor : Form
    {
        public FuseEditor()
        {
            InitializeComponent();
            panelEffect.Visible = false;

            comboType.Items.Clear();

            IEnumerable<TimelineEvent.EType> eTypes = EnumExtend.EnumToList<TimelineEvent.EType>();
            foreach (TimelineEvent.EType t in eTypes)
                comboType.Items.Add(t.DescString());

            string infoPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"UTF Editor\Fuse Composer\effects.txt");

            if (File.Exists(infoPath))
            {
                CultureInfo ci = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                string[] lines = File.ReadAllLines(infoPath);

                // Syntax:
                // Name; Shape
                // Time; ScaleX, ScaleY, ScaleZ; RotX, RotY, RotZ; X, Y, Z
                // ;
                EffectData currentFX = new EffectData();
                EffectData.Keyframe lastKeyframe = new EffectData.Keyframe();
                bool inside = false;
                foreach (string l in lines)
                {
                    if (!inside)
                    {
                        string[] x = l.Split(';');
                        if (x.Length != 2) continue;
                        currentFX.Name = x[0].Trim();
                        currentFX.Shape = (EffectData.EShape)Enum.Parse(typeof(EffectData.EShape), x[1].ToUpper().Trim());

                        inside = true;
                    }
                    else
                    {
                        if (l == ";")
                        {
                            inside = false;
                            comboEffect.Items.Add(currentFX);
                            currentFX = new EffectData();
                            continue;
                        }

                        try
                        {
                            string[] elements = l.Split(';');
                            EffectData.Keyframe kf = lastKeyframe.Clone() as EffectData.Keyframe;

                            switch (elements.Length)
                            {
                                // Handles trailing ";" possibility
                                case 5:
                                case 4:
                                    string[] pos = elements[1].Split(',');

                                    kf.X = Single.Parse(pos[0]);
                                    kf.Y = Single.Parse(pos[1]);
                                    kf.Z = Single.Parse(pos[2]);
                                    goto case 3;
                                case 3:
                                    string[] rot = elements[2].Split(',');

                                    kf.RotX = Single.Parse(rot[0]);
                                    kf.RotY = Single.Parse(rot[1]);
                                    kf.RotZ = Single.Parse(rot[2]);
                                    goto case 2;
                                case 2:
                                    string[] scale = elements[3].Split(',');

                                    kf.ScaleX = Single.Parse(scale[0]);
                                    kf.ScaleY = Single.Parse(scale[1]);
                                    kf.ScaleZ = Single.Parse(scale[2]);
                                    goto case 1;
                                case 1:
                                    kf.Time = Single.Parse(elements[0]);
                                    break;
                                default:
                                    continue;
                            }

                            currentFX.Length = currentFX.Length >= kf.Time ? currentFX.Length : kf.Time;

                            currentFX.Keyframes.Add(kf);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                if (inside)
                    comboEffect.Items.Add(currentFX);

                Thread.CurrentThread.CurrentCulture = ci;
            }
        }

        private void btnFlip_Click(object sender, EventArgs e)
        {
            if (splitFuseEditor.Orientation == Orientation.Vertical)
                splitFuseEditor.Orientation = Orientation.Horizontal;
            else
                splitFuseEditor.Orientation = Orientation.Vertical;

            timeline1.Zoom = 1;
        }

        private void splitFuseEditor_SplitterMoved(object sender, SplitterEventArgs e)
        {
            timeline1.Focus();
        }

        private void FuseEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                e.SuppressKeyPress = true;
                switch (e.KeyCode)
                {
                    case Keys.Space:
                        timeline1.Focus();
                        break;
                    case Keys.Down:
                    case Keys.Right:
                        timeline1.SelectNext();
                        break;
                    case Keys.Up:
                    case Keys.Left:
                        timeline1.SelectPrevious();
                        break;
                    default:
                        e.SuppressKeyPress = false;
                        break;
                }
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            timeline1.PlayPause();
        }

        private void timeline1_Play(object sender, EventArgs e)
        {
            btnPlayPause.Image = global::UTFEditor.Properties.Resources.StopHS;
        }

        private void timeline1_Stop(object sender, EventArgs e)
        {
            btnPlayPause.Image = global::UTFEditor.Properties.Resources.PlayHS;
        }

        private void comboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelEffect.Visible = false;

            switch (comboType.SelectedItem.ToString())
            {
                case "start_effect":
                    panelEffect.Visible = true;
                    break;
            }
        }

        private TimelineEvent current = null;

        private void timeline1_ItemAdd(object sender, Timeline.ItemAddEventArgs e)
        {
            TimelineEvent te = new TimelineEvent();

            te.Timings.Add(e.At);
            timeline1.Items.Add(e.At, te);

            SelectEvent(te);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            SelectEvent(new TimelineEvent());
        }

        private void RefreshUI()
        {
            bool e = current != null;

            comboType.Enabled = e;
            if(e) comboType.SelectedIndex = comboType.Items.IndexOf(current.Type.DescString());

            timeline1.Invalidate();
        }

        private void btnEffectAddTiming_Click(object sender, EventArgs e)
        {
            float v;
            if (!Single.TryParse(txtEffectAddTiming.Text, out v)) return;
            AddTiming(v);
        }

        private void AddTiming(float v)
        {
            current.Timings.Add(v);
            lstEffectTimings.Items.Add(v);
            timeline1.Items.Add(v, current);
            EventListEvent ele = new EventListEvent(current, v);
            comboEvents.Items.Add(ele);

            timeline1.SelectedItem = new KeyValuePair<float, object>(v, current);
            comboEvents.SelectedItem = ele;

            RefreshUI();
        }

        private void btnEffectRemoveTiming_Click(object sender, EventArgs e)
        {
            DeleteTiming();
        }

        private void DeleteTiming()
        {
            if (lstEffectTimings.SelectedIndex >= 0)
            {
                float v = (float)lstEffectTimings.SelectedItem;
                current.Timings.Remove(v);
                lstEffectTimings.Items.Remove(v);
                timeline1.Items.Remove(v, current);

                foreach (object o in comboEvents.Items)
                {
                    EventListEvent e = o as EventListEvent;
                    if (e.Event == current && e.Time == v)
                    {
                        comboEvents.Items.Remove(e);
                        break;
                    }
                }

                RefreshUI();
            }
        }

        private void txtEffectAddTiming_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                float v;
                if (!Single.TryParse(txtEffectAddTiming.Text, out v)) return;
                AddTiming(v);
                e.SuppressKeyPress = true;
            }
        }

        private void lstEffectTimings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteTiming();
                e.SuppressKeyPress = true;
            }
        }

        private void comboEffect_TextUpdate(object sender, EventArgs e)
        {
            foreach (EffectData ef in comboEffect.Items)
            {
                if (ef.Name == comboEffect.Text)
                {
                    current.Effect = ef;
                    RefreshUI();
                    return;
                }
            }
        }

        private void comboEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboEffect.SelectedIndex >= 0)
            {
                current.Effect = comboEffect.SelectedItem as EffectData;
                RefreshUI();
            }
        }

        private void timeline1_SelectionChanged(object sender, Timeline.SelectionChangedEventArgs e)
        {
            SelectEvent(e.Selection.Value as TimelineEvent, e.Selection.Key);
        }

        private void SelectEvent(TimelineEvent ev)
        {
            float at = ev.Timings.Count > 0 ? ev.Timings[0] : -1;
            SelectEvent(ev, at);
        }

        private void SelectEvent(TimelineEvent ev, float at)
        {
            current = ev;
            lstEffectTimings.Items.Clear();

            if (current != null)
            {
                comboType.SelectedItem = current.Type;

                foreach (float f in current.Timings)
                    lstEffectTimings.Items.Add(f);

                comboEffect.SelectedItem = current.Effect;

                if (at >= 0)
                {
                    timeline1.SelectedItem = new KeyValuePair<float, object>(at, current);

                    EventListEvent currele = comboEvents.SelectedItem as EventListEvent;
                    if (currele == null || currele.Event != ev || currele.Time != at)
                    {
                        foreach (object o in comboEvents.Items)
                        {
                            EventListEvent e = o as EventListEvent;
                            if (e.Event == current && e.Time == at)
                            {
                                comboEvents.SelectedItem = e;
                                break;
                            }
                        }
                    }
                }
                else
                    timeline1.SelectedItem = new KeyValuePair<float, object>(0, null);

                // TODO: Hardpoints
            }

            RefreshUI();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (comboEvents.Items.Count == 0) return;

            if (comboEvents.SelectedIndex < comboEvents.Items.Count-1)
            {
                comboEvents.SelectedIndex++;
                EventListEvent ele = comboEvents.SelectedItem as EventListEvent;
                SelectEvent(ele.Event, ele.Time);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (comboEvents.Items.Count == 0) return;

            if (comboEvents.SelectedIndex > 0)
            {
                comboEvents.SelectedIndex--;
                SelectEvent((comboEvents.SelectedItem as EventListEvent).Event);
            }
        }

        private void timeline1_TimeAdd(object sender, Timeline.TimeAddEventArgs e)
        {
            AddTiming(e.At);
        }

        private void comboEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboEvents.SelectedIndex < comboEvents.Items.Count && comboEvents.SelectedIndex >= 0)
            {
                EventListEvent ele = comboEvents.SelectedItem as EventListEvent;
                SelectEvent(ele.Event, ele.Time);
            }
        }

        private void txtTags_TextChanged(object sender, EventArgs e)
        {
            string[] tags = txtTags.Text.Split(',');
            current.Tags.Clear();
            foreach (string t in tags)
            {
                string t2 = t.Trim().ToLower();
                current.Tags.Add(t2);
            }
        }
    }

    public class EventListEvent : IEquatable<EventListEvent>
    {
        private float time;
        private TimelineEvent ev;

        public TimelineEvent Event
        {
            get
            {
                return ev;
            }
        }

        public float Time
        {
            get
            {
                return time;
            }
        }

        public EventListEvent(TimelineEvent e, float t)
        {
            time = t;
            ev = e;
        }

        public override string ToString()
        {
            return time + " - " + ev.Name;
        }

        #region IEquatable<EventListEvent> Members

        public bool Equals(EventListEvent other)
        {
            if (other.Time == time && other.Event == ev)
                return true;
            else
                return false;
        }

        #endregion
    }

    public class TimelineEvent : ITimelineElement
    {
        public enum EType
        {
            [Description("start_effect")]
            EFFECT
        }

        public struct HardpointData
        {
            public string Name;
            public float X, Y, Z;
            public float RotX, RotY, RotZ;
        }

        public EType Type = EType.EFFECT;

        // Type EFFECT
        public EffectData Effect;
        public bool Attached = false;
        public List<float> Timings = new List<float>();
        public List<HardpointData> Hardpoints = new List<HardpointData>();

        public string Name
        {
            get
            {
                if (Effect != null)
                    return this.Effect.Name;
                else
                    return "<no name>";
            }
        }

        public float Length
        {
            get
            {
                if (Effect != null)
                    return this.Effect.Length;
                else
                    return 0;
            }
        }

        private List<string> tags = new List<string>();

        public List<string> Tags
        {
            get
            {
                return tags;
            }
        }
    }

    public class EffectData
    {
        public enum EShape
        {
            SPHERE,
            CYLINDER,
            CONE,
            CUBE
        }

        public class Keyframe : ICloneable
        {
            public float Time;

            public float X, Y, Z;
            public float RotX, RotY, RotZ;
            public float ScaleX = 1, ScaleY = 1, ScaleZ = 1;

            #region ICloneable Members

            public object Clone()
            {
                Keyframe kf = new Keyframe();
                kf.X = X;
                kf.Y = Y;
                kf.Z = Z;
                kf.RotX = RotX;
                kf.RotY = RotY;
                kf.RotZ = RotZ;
                kf.ScaleX = ScaleX;
                kf.ScaleY = ScaleY;
                kf.ScaleZ = ScaleZ;

                return kf;
            }

            #endregion
        }

        public EShape Shape;

        public List<Keyframe> Keyframes = new List<Keyframe>();

        public string Name;

        public float Length;

        public override string ToString()
        {
            return Name;
        }
    }

    // Largely based from code by Luke Foust
    // http://blog.spontaneouspublicity.com/post/2008/01/17/Associating-Strings-with-enums-in-C.aspx

    public static class EnumExtend
    {
        public static string DescString(this Enum e)
        {
            FieldInfo fi = e.GetType().GetField(e.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return e.ToString();
        }

        public static IEnumerable<T> EnumToList<T>()
        {
            Type enumType = typeof(T);

            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            Array enumValArray = Enum.GetValues(enumType);
            List<T> enumValList = new List<T>(enumValArray.Length);

            foreach (int val in enumValArray)
            {
                enumValList.Add((T)Enum.Parse(enumType, val.ToString()));
            }

            return enumValList;
        }
    }
}
