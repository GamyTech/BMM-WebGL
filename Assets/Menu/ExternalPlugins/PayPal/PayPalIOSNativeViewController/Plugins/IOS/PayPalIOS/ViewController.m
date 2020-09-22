//
//  ViewController.m
//  PayPalIOSIntegration
//
//  Created by GamyTech on 09/03/2016.
//  Copyright © 2016 GamyTech. All rights reserved.
//

#import "ViewController.h"
#import "PayPalMobile.h"


@interface ViewController ()

    @property(nonatomic, strong, readwrite) PayPalConfiguration *payPalConfig;

@end

static NSString *clientID = @"";

@implementation ViewController

+(void)setClientID:(NSString *)client{
    clientID = client;
}

-(void)pay{


    NSString *s = [NSString stringWithFormat:@"viewcontroller : price : %@", self.price];
    NSLog(s);
    s = [NSString stringWithFormat:@"viewcontroller : email : %@", self.email];
    NSLog(s);
    s = [NSString stringWithFormat:@"viewcontroller : item : %@", self.item];
    NSLog(s);
    
    PayPalItem *item = [PayPalItem itemWithName:self.item
                                    withQuantity:1
                                       withPrice:[NSDecimalNumber decimalNumberWithString:self.price]
                                    withCurrency:@"USD"
                                         withSku:@""];
    
    NSArray *items = @[item];
    NSDecimalNumber *subtotal = [PayPalItem totalPriceForItems:items];
    
    // Optional: include payment details
    NSDecimalNumber *shipping = [[NSDecimalNumber alloc] initWithString:@"0"];
    NSDecimalNumber *tax = [[NSDecimalNumber alloc] initWithString:@"0"];
    PayPalPaymentDetails *paymentDetails =
    [PayPalPaymentDetails paymentDetailsWithSubtotal:subtotal withShipping:shipping withTax:tax];
    
    NSDecimalNumber *total = [[subtotal decimalNumberByAdding:shipping] decimalNumberByAdding:tax];
    
    PayPalPayment *payment = [[PayPalPayment alloc] init];
    payment.amount = total;
    payment.currencyCode = @"USD";
    payment.shortDescription = self.item;
    payment.items = items;
    payment.paymentDetails = paymentDetails;
    
    PayPalPaymentViewController *paymentViewController =
    [[PayPalPaymentViewController alloc] initWithPayment:payment configuration:self.payPalConfig delegate:self];
    
    [self presentViewController:paymentViewController animated:YES completion:nil];
}


- (void)payPalPaymentViewController:(PayPalPaymentViewController  *)paymentViewController didCompletePayment:(PayPalPayment *)completedPayment {
    NSLog(@"PayPal Payment Success!");
    
    self.resultText = [completedPayment description];
    NSLog(self.resultText);
    
    NSDictionary *dicpayments = [[NSDictionary alloc] initWithDictionary:completedPayment.confirmation[@"response"]];
    NSString *paymentKey = [dicpayments objectForKey:@"id" ];
    NSString *createdTime = [dicpayments objectForKey:@"create_time" ];
    NSString *state = [dicpayments objectForKey:@"state" ];
    
    
    NSString * responseToUnity = [NSString stringWithFormat:@"%@:%@:%@:%@:%@",@"maor",self.price,paymentKey,createdTime,state];
    const char * unity = [responseToUnity UTF8String];
    
    UnitySendMessage("GTPluginBridge", "PayPal_Callback", unity);
    
    [self dismissViewControllerAnimated:YES completion:nil];
}


- (void)payPalPaymentDidCancel:(PayPalPaymentViewController *)paymentViewController {
    NSLog(@"PayPal Payment Canceled");
    self.resultText = nil;
    UnitySendMessage("GTPluginBridge", "PayPal_Callback", "");
    [self dismissViewControllerAnimated:YES completion:nil];
}


- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}


-(void)viewDidAppear:(BOOL)animated{
    
    if(!self.fromWhereString)
    {
        self.fromWhereString = YES;
    }
    else
    {
        [self dismissViewControllerAnimated:NO completion:nil];
    }
}
//@"AUsl7M2gkCdKBxIOb3YLb0YOjtMAnqWRzqrYmNWZCeOSaJLBCjGMDq1eeSL_Qvj2A4iwYWB65RthzJUg"

- (void)viewDidLoad {
    [super viewDidLoad];
    
    [PayPalMobile initializeWithClientIdsForEnvironments:@{PayPalEnvironmentProduction : clientID ,PayPalEnvironmentSandbox : @"YOUR_CLIENT_ID_FOR_SANDBOX"}];
    
    [PayPalMobile preconnectWithEnvironment:PayPalEnvironmentProduction];

    self.fromWhereString = NO;
    
    NSLog(@"PayPal Controlller Called");
    
    self.environment = PayPalEnvironmentProduction;
    
    _payPalConfig.acceptCreditCards = NO;
    
    _payPalConfig.merchantName = @"GamyTech";
    _payPalConfig.merchantPrivacyPolicyURL =
    [NSURL URLWithString:@"https://www.paypal.com/webapps/mpp/ua/privacy-full"];
    _payPalConfig.merchantUserAgreementURL =
    [NSURL URLWithString:@"https://www.paypal.com/webapps/mpp/ua/useragreement-full"];
    
    // Setting the languageOrLocale property is optional.
    _payPalConfig.languageOrLocale = [NSLocale preferredLanguages][0];
    
    // Setting the payPalShippingAddressOption property is optional.
    // See PayPalConfiguration.h for details.
    _payPalConfig.payPalShippingAddressOption = PayPalShippingAddressOptionPayPal;

    
    [self pay];
}

@end



