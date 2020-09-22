//
//  CardViewController.m
//  Unity-iPhone
//
//  Created by gamytech-mac on 21/03/2016.
//
//
#import "CardIO.h"
#import "CardViewController.h"
#import "IOSBridge.h"
@interface CardViewController () <CardIOPaymentViewControllerDelegate>

@end

@implementation CardViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    if (![CardIOUtilities canReadCardWithCamera]) {
        // Hide your "Scan Card" button, or take other appropriate action...
    }
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [CardIOUtilities preload];
}

- (void)scanCard{
    CardIOPaymentViewController *scanViewController = [[CardIOPaymentViewController alloc] initWithPaymentDelegate:self];
    scanViewController.modalPresentationStyle = UIModalPresentationFormSheet;
    [self presentViewController:scanViewController animated:YES completion:nil];
}

- (void)cardIOView:(CardIOView *)cardIOView didScanCard:(CardIOCreditCardInfo *)info {
    if (info) {
        // The full card number is available as info.cardNumber, but don't log that!
        NSLog(@"Received card info. Number: %@, expiry: %02i/%i, cvv: %@.", info.cardNumber, info.expiryMonth, info.expiryYear, info.cvv);
        NSString * responseToUnity = [NSString stringWithFormat:@"%@:%lu:%lu:%@",info.cardNumber,(unsigned long)info.expiryMonth,(unsigned long)info.expiryYear,info.cvv];
        const char * unity = [responseToUnity UTF8String];
            UnitySendMessage("GTPluginBridge", "PayPal_Callback", unity);
        // Use the card info...
    }
    else {
        NSLog(@"User cancelled payment info");
        // Handle user cancellation here...
    }
    
    [cardIOView removeFromSuperview];
}

- (void)userDidCancelPaymentViewController:(CardIOPaymentViewController *)scanViewController {
    NSLog(@"User canceled payment info");
    // Handle user cancellation here...
    UnitySendMessage("GTPluginBridge", "PayPal_Callback", "");
    [scanViewController dismissViewControllerAnimated:YES completion:nil];
    [self dismissViewControllerAnimated:YES completion:nil];
}
#pragma mark - CardIOPaymentViewControllerDelegate

- (void)userDidProvideCreditCardInfo:(CardIOCreditCardInfo *)info inPaymentViewController:(CardIOPaymentViewController *)scanViewController {
    // The full card number is available as info.cardNumber, but don't log that!
    NSLog(@"Received card info. Number: %@, expiry: %02i/%i, cvv: %@.", info.cardNumber, info.expiryMonth, info.expiryYear, info.cvv);
    NSString * responseToUnity = [NSString stringWithFormat:@"%@:%lu:%lu:%@",info.cardNumber,(unsigned long)info.expiryMonth,(unsigned long)info.expiryYear,info.cvv];
    const char * unity = [responseToUnity UTF8String];
    UnitySendMessage("GTPluginBridge", "PayPal_Callback", unity);
    // Use the card info...
    [scanViewController dismissViewControllerAnimated:YES completion:nil];
    [self dismissViewControllerAnimated:YES completion:nil];
}

/*
 #pragma mark - Navigation
 
 // In a storyboard-based application, you will often want to do a little preparation before navigation
 - (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
 // Get the new view controller using [segue destinationViewController].
 // Pass the selected object to the new view controller.
 }
 */


@end
