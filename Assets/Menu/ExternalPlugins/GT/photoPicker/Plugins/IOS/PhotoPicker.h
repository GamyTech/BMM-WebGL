//
//  PhotoPicker.h
//  PhotoPicker
//
//  Created by gamytech-mac on 14/12/2017.
//  Copyright © 2017 GamyTech. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface PhotoPicker : NSObject <UIImagePickerControllerDelegate, UINavigationControllerDelegate>

// filename: name of the picture file
// callback: name of unity method to call with the result
// gameObject: name of the gameObject that has the script with the callback
- (void)selectPictureWithFilename:(NSString*)filename callback:(NSString*)callback gameObject:(NSString*)gameObject;
- (void)takePictureWithFilename:(NSString*)filename callback:(NSString*)callback gameObject:(NSString*)gameObject;

@end

extern "C" UIViewController* UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);
