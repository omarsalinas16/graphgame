﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;


/// <summary>
/// This class is where the transformation, rotations and  scale happens
/// It trie to join [FormBehaviour] and [PlayerController]
/// </summary>
public class TransformationsHelper
{
    private Vector3 translateLimitsMin = new Vector3(-2, -2, -2);
    private Vector3 translateLimitsMax = new Vector3(2, 2, 2);
    private Vector3 scaleLimitsMin = new Vector3(1, 1, 1);
    private Vector3 scaleLimitsMax = new Vector3(3, 3, 3);

    private float scaleOffsetFix = 0.05f;
    private Vector3 scaleOffsetForSolvedScale;

    private float interpolationDuration = 2.0f;        
    private Ease interpolationEase = Ease.InOutCubic;

    private Transform activeForm;
    private float stepScale;
    private float stepTranslate;
    public TransformationsHelper(Transform activeForm, float stepScale, float stepTranslate) {
        this.activeForm = activeForm;
        this.stepScale = stepScale;
        this.stepTranslate = stepTranslate;
        scaleOffsetForSolvedScale = new Vector3(scaleOffsetFix, scaleOffsetFix, scaleOffsetFix);
    }

    public float incrementScaleX(int direction, float currentValue)
    {
        return incrementScale(direction, currentValue, activeForm.localScale.x, scaleLimitsMin.x, scaleLimitsMax.x);
    }

    public float incrementScaleY(int direction, float currentValue)
    {
        return incrementScale(direction, currentValue, activeForm.localScale.y, scaleLimitsMin.y, scaleLimitsMax.y);
    }

    public float incrementScaleZ(int direction, float currentValue)
    {        
        return incrementScale(direction, currentValue, activeForm.localScale.z, scaleLimitsMin.z, scaleLimitsMax.z);
    }

    public float incrementPositionX(int direction, float currentValue)
    {
        return incrementPosition(direction, currentValue, activeForm.localPosition.x, translateLimitsMin.x, translateLimitsMax.x);
    }

    public float incrementPositionY(int direction, float currentValue)
    {
        return incrementPosition(direction, currentValue, activeForm.localPosition.y, translateLimitsMin.y, translateLimitsMax.y);
    }

    public float incrementPositionZ(int direction, float currentValue)
    {
        return incrementPosition(direction, currentValue, activeForm.localPosition.z, translateLimitsMin.z, translateLimitsMax.z);
    }
    
    public float incrementPosition(int direction,float currentValue, float currentFormPosition, float min, float max) {
        var wantedValue = currentValue + stepScale * direction;
        // TODO: i would prefer to no longer have the limits in a vector3 and only hava a translateLimitMin and translateLimitMax
        var resultFormPosition = currentFormPosition + wantedValue;
        if (resultFormPosition > max)
        {
            wantedValue = max - currentFormPosition;
        }
        else if (resultFormPosition < min)
        {
            wantedValue = min - currentFormPosition;
        }
        return wantedValue;
    }

    public float incrementScale(int direction, float currentValue, float currentFormScale, float min, float max)
    {
        var wantedValue = currentValue + stepTranslate * direction;
        // TODO: i would prefer to no longer have the limits in a vector3 and only hava a translateLimitMin and translateLimitMax
        var resultFormPosition = currentFormScale * wantedValue;
        Debug.Log(resultFormPosition);
        if (currentFormScale != 0) {
            if (resultFormPosition % stepTranslate != 0)
            {
                resultFormPosition = (float)Math.Round(resultFormPosition);
                wantedValue = resultFormPosition / currentFormScale;
            }

            if (resultFormPosition > max)
            {
               wantedValue = max / currentFormScale;                
            }
            else if (resultFormPosition < min)
            {
               wantedValue = min / currentFormScale;             
            }
        }
        
        
        return wantedValue;
    }

    public void setFormToStartState(Level level) {
        resetForm();
        setTargetScale(level.startScale);
        setTargetTranslate(level.startPosition);
        foreach (Level.Rotation r in level.startRotations) {
            setTargetRotation(r.Value, r.Axis);
        }
    }

    public void setFormToSolvedState(Level level)
    {
        resetForm();
        setTargetScale(level.scale + scaleOffsetForSolvedScale, false);
        setTargetTranslate(level.position, false);
        foreach (Level.Rotation r in level.rotations)
        {
            setTargetRotation(r.Value, r.Axis);
        }
    }

    private void resetForm()
    {
        activeForm.position = new Vector3(0, 0, 0);
        activeForm.rotation = Quaternion.Euler(0, 0, 0);
        activeForm.localScale = new Vector3(1, 1, 1);
    }

    public void setTargetTranslate(Vector3 t, bool animate = true)
    {
        Vector3 targetTranslate = activeForm.localPosition;
        targetTranslate.x += t.x;
        targetTranslate.y += t.y;
        targetTranslate.z += t.z;
        /*targetTranslate.x = Mathf.Clamp(targetTranslate.x, translateLimitsMin.x, translateLimitsMax.x);
        targetTranslate.y = Mathf.Clamp(targetTranslate.y, translateLimitsMin.y, translateLimitsMax.y);
        targetTranslate.z = Mathf.Clamp(targetTranslate.z, translateLimitsMin.z, translateLimitsMax.z);*/
        if (activeForm)
        {
            if (animate)
            {
                activeForm.DOLocalMove(targetTranslate, interpolationDuration).SetEase(interpolationEase);
            }
            else {
                activeForm.localPosition = targetTranslate;
            }            
        }
    }

    public void setTargetScale(Vector3 s, bool animate = true)
    {
        Vector3 targetScale = activeForm.localScale;
        targetScale.x *= s.x;
        targetScale.y *= s.y;
        targetScale.z *= s.z;        
        
        /*targetScale.x = Mathf.Clamp(targetScale.x, scaleLimitsMin.x, scaleLimitsMax.x);
        targetScale.y = Mathf.Clamp(targetScale.y, scaleLimitsMin.y, scaleLimitsMax.y);
        targetScale.z = Mathf.Clamp(targetScale.z, scaleLimitsMin.z, scaleLimitsMax.z);*/

        if (activeForm)
        {
            //activeForm.DOScaleX(targetScale.x, interpolationDuration).SetEase(interpolationEase);
            //activeForm.DOScaleY(targetScale.y, interpolationDuration).SetEase(interpolationEase);
            //activeForm.DOScaleZ(targetScale.z, interpolationDuration).SetEase(interpolationEase);
            if (animate)
            {
                activeForm.DOScale(targetScale, interpolationDuration).SetEase(interpolationEase);
            }
            else {
                activeForm.localScale = targetScale;
            }
            
        }
    }

    public void setTargetRotation(float rotation, GameInputController.ROTATION_AXIS axis)
    {

        if (activeForm)
        {

            Vector3 currentRotation = new Vector3();
            Quaternion r;
            switch (axis)
            {
                case GameInputController.ROTATION_AXIS.X:
                    activeForm.RotateAround(activeForm.position, activeForm.right, rotation);
                    r = activeForm.rotation;
                    currentRotation = new Vector3(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);
                    //activeForm.RotateAround(activeForm.position, activeForm.right, -rotation);
                    break;
                case GameInputController.ROTATION_AXIS.Y:
                    activeForm.RotateAround(activeForm.position, activeForm.up, rotation);
                    r = activeForm.rotation;
                    currentRotation = new Vector3(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);
                    //activeForm.RotateAround(activeForm.position, activeForm.up, -rotation);
                    break;
                case GameInputController.ROTATION_AXIS.Z:
                    activeForm.RotateAround(activeForm.position, activeForm.forward, rotation);
                    r = activeForm.rotation;
                    currentRotation = new Vector3(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);
                    //activeForm.RotateAround(activeForm.position, activeForm.forward, -rotation);
                    break;
            }
            //activeForm.DORotate(currentRotation, interpolationDuration, RotateMode.FastBeyond360).SetEase(interpolationEase);
            // TODO: make the animarion great again

        }
    }
}

