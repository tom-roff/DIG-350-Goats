// Allows iOS to play sound without having to unlock "silent button"
// https://forum.unity.com/threads/problem-with-mute-button-ios-silent-button.387579/
#import "UnityAppController.h"
#import "AVFoundation/AVFoundation.h"
 
@interface MyUnityAppController: UnityAppController {}
 
-(void)setAudioSession;
 
@end
 
@implementation MyUnityAppController
 
-(void) startUnity: (UIApplication*) application
{
    NSLog(@"MyUnityAppController startUnity");
    [super startUnity: application];  //call the super.
    [self setAudioSession];
}
 
- (void)setAudioSession
{
    NSLog(@"MyUnityAppController Set audiosession");
    AVAudioSession *audioSession = [AVAudioSession sharedInstance];
    // [audioSession setCategory:AVAudioSessionCategoryAmbient error:nil];
    // [audioSession setActive:YES error:nil];
    // https://forum.unity.com/threads/problem-with-mute-button-ios-silent-button.387579/#post-3027853
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback error:nil];
    [[AVAudioSession sharedInstance] setActive:YES error:nil];
}
 
@end
 
IMPL_APP_CONTROLLER_SUBCLASS(MyUnityAppController)