//
//  IOSBridge.m
//  test
//
//  Created by GamyTech on 02/03/2016.
//  Copyright © 2016 GamyTech. All rights reserved.
//

#import "IOSBridge.h"
#import "ViewController.h"
#import "CardViewController.h"

static int price;
static NSString *email;
static NSString *paymentId;
static NSString *cliendId;

@implementation IOSBridge
UIViewController *UnityGetGLViewController();
@end


@implementation Delegate

-(id)init{
    return self;
}

@end
static ViewController *PPmvc;
static CardViewController *Cardmvc;
static Delegate *delegate;

extern "C"{
    
    void _AddNotification(const char* title, const char* body,
                          const char* cancelLabel,const char* firstLabel,
                          const char* secondLabel){
        
        NSLog(@"Xcode : Alert Button clicked");
        
        if(delegate ==nil){
            delegate = [[Delegate alloc]init];
        }
        
        UIAlertView *view = [[UIAlertView alloc]
                             initWithTitle:[NSString stringWithUTF8String:title]
                             message:[NSString stringWithUTF8String:body] delegate:nil
                             cancelButtonTitle:[NSString stringWithUTF8String:cancelLabel]
                             otherButtonTitles:[NSString stringWithUTF8String:firstLabel],
                             [NSString stringWithUTF8String:secondLabel],
                             nil];
        
        [view show];
        
    }
    
    void _InitPaypal(const char *client){
        NSLog(@"Xcode : _InitPaypal");
        cliendId = [NSString stringWithUTF8String:client];
        [ViewController setClientID:cliendId];
    }
    
    void _ChangePaypalViewController(const char* prodName,int _price,const char* email){
        NSLog(@"Xcode : View Button clicked");
        price = _price;
        NSString *s = [NSString stringWithFormat:@"Xcode : price is %d", price];
        NSLog(s);
        PPmvc = [[ViewController alloc]init];

        PPmvc.price = [NSString stringWithFormat:@"%d",_price];
        PPmvc.email = [NSString stringWithUTF8String:email];
        PPmvc.item  = [NSString stringWithUTF8String:prodName];
        
        [UnityGetGLViewController() presentViewController:PPmvc animated:true completion:nil];

        [ PPmvc pay];
    }
    
    void _ChangeCardViewController(){
        NSLog(@"Xcode : View Button clicked");
        
        Cardmvc = [[CardViewController alloc] init];

        
        [UnityGetGLViewController() presentViewController:Cardmvc animated:true completion:nil];
        
        [ Cardmvc scanCard];
    }
    
}
