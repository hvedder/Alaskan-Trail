using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedLimb : MonoBehaviour {

	public bool animating;
	public int currentStep;
	public float stepTime;

	private List<AnimationStep> animationSteps = new List<AnimationStep>();
	private AnimationStep origin;

	private float posSpeed;
	private float rotSpeed;
	private float scaleSpeed;

	public bool loop;

    // Start is called before the first frame update
    void Start () {
        origin = new AnimationStep(transform.localPosition, transform.localEulerAngles, transform.localScale, 0);
    }

    // Update is called once per frame
    void Update () {
        if (!animating) {
        	return;
        }

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, origin.position + animationSteps[currentStep].position, posSpeed * Time.deltaTime);
        transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, origin.rotation + animationSteps[currentStep].rotation, rotSpeed * Time.deltaTime);
        transform.localScale = Vector3.MoveTowards(transform.localScale, origin.scale + animationSteps[currentStep].scale, scaleSpeed * Time.deltaTime);

        stepTime -= Time.deltaTime;

        if (stepTime <= 0) {
        	currentStep++;

        	if (currentStep >= animationSteps.Count) {
        		if (loop) {
        			currentStep = 0;

        			CalculateSpeeds(currentStep);
        		}
        		else {
        			animating = false;
        		}
        	}
        	else {
        		CalculateSpeeds(currentStep);
        	}
        }
    }

    public void Animate (List<AnimationStep> steps) {
    	currentStep = 0;
    	animationSteps = steps;
    	animating = true;

    	CalculateSpeeds(0);
    }

    public void Stop () {
    	animating = false;

    	transform.localPosition = origin.position;
    	transform.localEulerAngles = origin.rotation;
    	transform.localScale = origin.scale;
    }

    private void CalculateSpeeds (int step) {
    	stepTime = animationSteps[step].time;

    	if (animationSteps[step].time > 0) {
    		posSpeed = Vector3.Distance(transform.localPosition, origin.position + animationSteps[step].position) / animationSteps[step].time;
    		rotSpeed = Vector3.Distance(transform.localEulerAngles, origin.rotation + animationSteps[step].rotation)  / animationSteps[step].time;
    		scaleSpeed = Vector3.Distance(transform.localScale, origin.scale + animationSteps[step].scale) / animationSteps[step].time;
    	}
    	else {
    		posSpeed = 0;
    		rotSpeed = 0;
    		scaleSpeed = 0;

    		transform.localPosition = origin.position + animationSteps[step].position;
    		transform.localEulerAngles = origin.rotation + animationSteps[step].rotation;
    		transform.localScale = origin.scale + animationSteps[step].scale;
    	}
    }

    private bool CheckForStepEnd () {
    	if (transform.localPosition != origin.position + animationSteps[currentStep].position) {
    		return false;
    	}
    	if (transform.localEulerAngles != origin.rotation + animationSteps[currentStep].rotation) {
    		return false;
    	}
    	if (transform.localScale != origin.scale + animationSteps[currentStep].scale) {
    		return false;
    	}

    	return true;
    }
}

public class AnimationStep {
	public Vector3 position;
	public Vector3 rotation;
	public Vector3 scale;
	public float time;

	public AnimationStep (float time) {
		this.position = Vector3.zero;
		this.rotation = Vector3.zero;
		this.scale = Vector3.zero;
		this.time = time;
	}

	public AnimationStep (Vector3 position, Vector3 rotation, Vector3 scale, float time) {
		this.position = position;
		this.rotation = rotation;
		this.scale = scale;
		this.time = time;
	}
}
