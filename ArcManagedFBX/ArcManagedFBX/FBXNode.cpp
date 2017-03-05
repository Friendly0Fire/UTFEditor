#include "Stdafx.h"
#include "FBXLine.h"
#include "FBXCamera.h"
#include "FBXSkeleton.h"
#include "FBXShape.h"
#include "FBXLODGroup.h"
#include "FBXLine.h"
#include "FBXBoundary.h"
#include "FBXLight.h"
#include "FBXNode.h"

using namespace ArcManagedFBX;
using namespace ArcManagedFBX::Types;
using namespace ArcManagedFBX::Utility;

using namespace System;
using namespace System::Text;

FBXNode::FBXNode() : FBXObject()
{

}

FBXNode::~FBXNode()
{

}

FBXNode::!FBXNode()
{
}

FBXNode::FBXNode(FbxNode* instance) : FBXObject(instance)
{
}

void FBXNode::AddChild(FBXNode^ node)
{
	ARC_CHECK_AND_THROW(node == nullptr || node->GetFBXNode() == nullptr, "This FBX node has not been properly initialized. Check and try again")

	this->GetFBXNode()->AddChild(node->GetFBXNode());
}

int32 ArcManagedFBX::FBXNode::GetDefaultNodeAttributeIndex()
{
	ARC_CHECK_AND_THROW(this->GetFBXNode() == nullptr, "This FBX node has not been properly initialized. Check and try again")

	return this->GetFBXNode()->GetDefaultNodeAttributeIndex();
}

int32 ArcManagedFBX::FBXNode::GetNodeAttributeCount()
{
	ARC_CHECK_AND_THROW(this->GetFBXNode() == nullptr, "This FBX node has not been properly initialized. Check and try again")

	return this->GetFBXNode()->GetNodeAttributeCount();
}

FBXNodeAttribute^ ArcManagedFBX::FBXNode::GetNodeAttribute()
{
	ARC_CHECK_AND_THROW(this->GetFBXNode() == nullptr, "This FBX node has not been properly initialized. Check and try again")

	FbxNodeAttribute* attributeInstance = this->GetFBXNode()->GetNodeAttribute();

	// Be sure to cast the value properly into the right clr type otherwise we end up in a scenario where we have to instantiate new objects on the heap.
	switch(this->GetFBXNode()->GetNodeAttribute()->GetAttributeType())
	{
	case FbxNodeAttribute::EType::eLight:
			return FBXNodeAttribute::CreateNodeAttribute<FBXLight^>(attributeInstance);		
		break;

	case FbxNodeAttribute::EType::eBoundary:
			return FBXNodeAttribute::CreateNodeAttribute<FBXBoundary^>(attributeInstance);		
		break;

	case FbxNodeAttribute::EType::eMesh:
			return FBXNodeAttribute::CreateNodeAttribute<FBXMesh^>(attributeInstance);		
		break;

	case FbxNodeAttribute::EType::eSkeleton:
			return FBXNodeAttribute::CreateNodeAttribute<FBXSkeleton^>(attributeInstance);
		break;

	case FbxNodeAttribute::EType::eCamera:
			return FBXNodeAttribute::CreateNodeAttribute<FBXCamera^>(attributeInstance);
		break;

	case FbxNodeAttribute::EType::eLine:
			return FBXNodeAttribute::CreateNodeAttribute<FBXLine^>(attributeInstance);
		break;

	case FbxNodeAttribute::EType::eShape:
			return FBXNodeAttribute::CreateNodeAttribute<FBXShape^>(attributeInstance);
		break;

	case FbxNodeAttribute::EType::eLODGroup:
			return FBXNodeAttribute::CreateNodeAttribute<FBXLODGroup^>(attributeInstance);
		break;
	}

	return gcnew FBXNodeAttribute(this->GetFBXNode()->GetNodeAttribute());
}

FBXVector ArcManagedFBX::FBXNode::EvaluateLocalTranslation(FBXTime time, EPivotSet pivotSet, bool applyTarget, bool forceEval)
{
	return FBXVector(this->GetFBXNode()->EvaluateLocalTranslation(time, (FbxNode::EPivotSet)pivotSet, applyTarget, forceEval));
}

FBXVector ArcManagedFBX::FBXNode::EvaluateLocalRotation(FBXTime time, EPivotSet pivotSet, bool applyTarget, bool forceEval)
{
	return FBXVector(this->GetFBXNode()->EvaluateLocalRotation(time, (FbxNode::EPivotSet)pivotSet, applyTarget, forceEval));
}

FBXVector ArcManagedFBX::FBXNode::EvaluateLocalScaling(FBXTime time, EPivotSet pivotSet, bool applyTarget, bool forceEval)
{
	return FBXVector(this->GetFBXNode()->EvaluateLocalScaling(time, (FbxNode::EPivotSet)pivotSet, applyTarget, forceEval));
}

FBXMatrix ArcManagedFBX::FBXNode::EvaluateLocalTransform(FBXTime time, EPivotSet pivotSet, bool applyTarget, bool forceEval)
{
	return FBXMatrix(this->GetFBXNode()->EvaluateLocalTransform(time, (FbxNode::EPivotSet)pivotSet, applyTarget, forceEval));
}

FBXMatrix ArcManagedFBX::FBXNode::EvaluateGlobalTransform(FBXTime time, EPivotSet pivotSet, bool applyTarget, bool forceEval)
{
	return FBXMatrix(this->GetFBXNode()->EvaluateGlobalTransform(time, (FbxNode::EPivotSet)pivotSet, applyTarget, forceEval));
}

FBXVector ArcManagedFBX::FBXNode::GetGeometricTranslation(EPivotSet pivotSet)
{
	return FBXVector(this->GetFBXNode()->GetGeometricTranslation((fbxsdk_2015_1::FbxNode::EPivotSet)pivotSet));
}

FBXVector ArcManagedFBX::FBXNode::GetGeometricRotation(EPivotSet pivotSet)
{
	return FBXVector(this->GetFBXNode()->GetGeometricRotation((fbxsdk_2015_1::FbxNode::EPivotSet)pivotSet));
}

FBXVector ArcManagedFBX::FBXNode::GetGeometricScaling(EPivotSet pivotSet)
{
	return FBXVector(this->GetFBXNode()->GetGeometricScaling((fbxsdk_2015_1::FbxNode::EPivotSet)pivotSet));
}

FBXMatrix ArcManagedFBX::FBXNode::GetGeometricMatrix(EPivotSet pivotSet)
{
	fbxsdk_2015_1::FbxVector4 t = this->GetFBXNode()->GetGeometricTranslation((fbxsdk_2015_1::FbxNode::EPivotSet)pivotSet);
	fbxsdk_2015_1::FbxVector4 r = this->GetFBXNode()->GetGeometricRotation((fbxsdk_2015_1::FbxNode::EPivotSet)pivotSet);
	fbxsdk_2015_1::FbxVector4 s = this->GetFBXNode()->GetGeometricScaling((fbxsdk_2015_1::FbxNode::EPivotSet)pivotSet);

	fbxsdk_2015_1::FbxMatrix m;
	m.SetTRS(t, r, s);

	return FBXMatrix(m);
}

int ArcManagedFBX::FBXNode::GetMaterialCount()
{
	return this->GetFBXNode()->GetMaterialCount();
}

FBXSurfaceMaterial^ ArcManagedFBX::FBXNode::GetMaterial(int index)
{
	return gcnew FBXSurfaceMaterial(this->GetFBXNode()->GetMaterial(index));
}

int ArcManagedFBX::FBXNode::GetMaterialIndex(const char * name)
{
	return this->GetFBXNode()->GetMaterialIndex(name);
}

FBXNodeAttribute^ ArcManagedFBX::FBXNode::SetNodeAttribute(FBXNodeAttribute^ attribute)
{
	ARC_CHECK_AND_THROW(this->GetFBXNode() == nullptr, "This FBX node has not been properly initialized. Check and try again")

	return gcnew FBXNodeAttribute(this->GetFBXNode()->SetNodeAttribute(attribute->GetFBXNodeAttribute()));
}

ArcManagedFBX::Types::EShadingMode ArcManagedFBX::FBXNode::GetShadingMode()
{
	ARC_CHECK_AND_THROW(this->GetFBXNode() == nullptr, "This FBX node has not been properly initialized. Check and try again")

	return (EShadingMode)this->GetFBXNode()->GetShadingMode();
}

void ArcManagedFBX::FBXNode::SetShadingMode(EShadingMode mode)
{
	this->GetFBXNode()->SetShadingMode((FbxNode::EShadingMode)mode);
}

bool ArcManagedFBX::FBXNode::GetVisibility()
{
	return this->GetFBXNode()->GetVisibility();
}

void ArcManagedFBX::FBXNode::SetVisibility(bool isVisible)
{
	this->GetFBXNode()->SetVisibility(isVisible);
}

ArcManagedFBX::FBXVector ArcManagedFBX::FBXNode::GetTargetUpVector()
{
	return FBXVector(this->GetFBXNode()->GetTargetUpVector());
}

void ArcManagedFBX::FBXNode::SetTargetUpVector(FBXVector vector)
{

}

FBXNode^ ArcManagedFBX::FBXNode::GetTargetUp()
{
	return gcnew FBXNode(this->GetFBXNode()->GetTargetUp());
}

void ArcManagedFBX::FBXNode::SetTargetUp(FBXNode^ pNode)
{
	this->GetFBXNode()->SetTargetUp(pNode->GetFBXNode());
}

ArcManagedFBX::FBXVector ArcManagedFBX::FBXNode::GetPostTargetRotation()
{
	return FBXVector(this->GetFBXNode()->GetPostTargetRotation());
}

void ArcManagedFBX::FBXNode::SetPostTargetRotation(FBXVector vector)
{
	// TODO:
}

FBXNode^ ArcManagedFBX::FBXNode::GetTarget()
{
	return gcnew FBXNode(this->GetFBXNode()->GetTarget());
}

void ArcManagedFBX::FBXNode::SetTarget(FBXNode^ node)
{
	this->GetFBXNode()->SetTarget(node->GetFBXNode());
}

FBXNode^ ArcManagedFBX::FBXNode::FindChild(String^ name, bool recursive, bool initial)
{
	const char* nameNative = StringHelper::ToNative(name);
	return gcnew FBXNode(this->GetFBXNode()->FindChild(nameNative,recursive,initial));
}

FBXNode^ ArcManagedFBX::FBXNode::GetChild(int32 pIndex)
{
	return gcnew FBXNode(this->GetFBXNode()->GetChild(pIndex));
}

void ArcManagedFBX::FBXNode::RemoveChild(FBXNode^ Node)
{
	this->GetFBXNode()->RemoveChild(Node->GetFBXNode());
}

bool ArcManagedFBX::FBXNode::SetDefaultNodeAttributeIndex(int32 index, FBXStatus^ status)
{
	return this->GetFBXNode()->SetDefaultNodeAttributeIndex(index,status->GetInstance());
}

int32 ArcManagedFBX::FBXNode::GetChildCount(bool recursive)
{
	ARC_CHECK_AND_THROW(this->GetFBXNode() == nullptr, "This FBX node has not been properly initialized. Check and try again")

	return this->GetFBXNode()->GetChildCount();
}
