//
//  ViewController.h
//  PayPalIOSIntegration
//
//  Created by GamyTech on 09/03/2016.
//  Copyright © 2016 GamyTech. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ViewController : UIViewController

- (void)setPayPalEnvironment:(NSString *)environment;
- (void)pay;

+(void)setPayment:(NSString *)price: (NSString *)name;
+(void)setClientID:(NSString *)client;

@property(nonatomic, strong, readwrite) NSString *environment;
@property(nonatomic, strong, readwrite) NSString *resultText;

@property(nonatomic,readwrite) Boolean *fromWhereString;
@property(nonatomic,readwrite) NSString *price;
@property(nonatomic,readwrite) NSString *item;
@property(nonatomic,readwrite) NSString *email;
@property(nonatomic,readwrite) NSString *paymentID;


@end

