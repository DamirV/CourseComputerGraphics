#version 430

in vec3 origin, direction;
out vec4 outputColor;



struct Sphere {
    vec3 position;
    float radius;
    vec3 color;
};

struct Ray {
    vec3 origin;
    vec3 direction;
};

struct RayNode {
    Ray ray;
    vec3 color;
    int depth;
};

struct HitInfo {
    bool hitDetected;
    vec3 hitPoint;
    vec3 surfaceNormal;
    float distance;
    int objectid;
    vec3 color;
};
const int sphereNumber = 2;
Sphere spheres[7];
const int Max_Depth = 5;
const int Max_Nodes = 64;
float coeff = 1;
RayNode rayNode[Max_Nodes];

void sphereIntersect(Ray ray, int objectid, inout HitInfo hitInfo) {
    Sphere sphere = spheres[objectid];
    vec3 trackToSphere = ray.origin - sphere.position;
    float a = dot(ray.direction, ray.direction);
    float b = 2 * dot(trackToSphere, ray.direction);
    float c = dot(trackToSphere, trackToSphere) - sphere.radius * sphere.radius;
    float discriminant = b * b - 4.0 * a * c;    

    if (discriminant > 0.0) {
	float distance = (-b - sqrt(discriminant)) / (2.0 * a);
	if (distance > 0.0001 && hitInfo.objectid ==0 && (distance < hitInfo.distance && hitInfo.hitDetected || !hitInfo.hitDetected)) {
	    hitInfo.distance = distance;
	    hitInfo.hitPoint = ray.origin + ray.direction *  hitInfo.distance;
  	    hitInfo.surfaceNormal = normalize(hitInfo.hitPoint - sphere.position);
	    hitInfo.hitDetected = true;
	    hitInfo.objectid = objectid;
	    hitInfo.color = sphere.color;
	}
    }
}

vec3 iterativeRayTrace(Ray ray) {
    Sphere sphere;
    sphere.position = vec3(-0.4, 0.0, -0.2);
    sphere.radius = 0.5;
    sphere.color = vec3(0.3, 0.1, 0.7);	
    int numberOfNodes = 6, currentNodeIndex = 0;
    spheres[0] = sphere;
    sphere.position = vec3(0.3, 0.0, -1.0);
    sphere.radius = 1.0;
    sphere.color = vec3(0.6, 0.5, 0.2);	
    spheres[1] = sphere;

	sphere.position = vec3(-65, 0.0, -50);
    sphere.radius = 50;
    sphere.color = vec3(1, 1, 1);	
    spheres[2] = sphere;

	sphere.position = vec3(65, 0.0, -50);
    sphere.radius = 50;
    sphere.color = vec3(1, 1, 1);	
    spheres[3] = sphere;

	sphere.position = vec3(0, -65, -50);
    sphere.radius = 50;
    sphere.color = vec3(1, 1, 1);	
    spheres[4] = sphere;

	sphere.position = vec3(0, 65, -50);
    sphere.radius = 50;
    sphere.color = vec3(1, 1, 1);	
    spheres[5] = sphere;

	sphere.position = vec3(0, 0, -50);
    sphere.radius = 60;
    sphere.color = vec3(1, 1, 1);	
    spheres[6] = sphere;

    rayNode[currentNodeIndex].ray = ray;
    rayNode[currentNodeIndex].depth = 0;

    while (currentNodeIndex < numberOfNodes) {
	HitInfo hitInfo;
	hitInfo.hitDetected = false;
	sphereIntersect(ray, 0, hitInfo);
    sphereIntersect(ray, 1, hitInfo);
	sphereIntersect(ray, 2, hitInfo);
	sphereIntersect(ray, 3, hitInfo);
	sphereIntersect(ray, 4, hitInfo);
	sphereIntersect(ray, 5, hitInfo);
	sphereIntersect(ray, 6, hitInfo);

	if (hitInfo.hitDetected) {
   	coeff = (dot(ray.direction, hitInfo.surfaceNormal) * dot(ray.direction, hitInfo.surfaceNormal)) / ((dot(ray.direction, ray.direction)) * (dot(hitInfo.surfaceNormal, hitInfo.surfaceNormal)));

	rayNode[currentNodeIndex].color = hitInfo.color * coeff;
	}
	else rayNode[currentNodeIndex].color = vec3(0, 0, 0);
	currentNodeIndex++;
    }
    return rayNode[0].color;
}

	

void main()
{

    Ray ray = Ray(origin, direction);
    outputColor = vec4(iterativeRayTrace(ray), 1.0) ;
}