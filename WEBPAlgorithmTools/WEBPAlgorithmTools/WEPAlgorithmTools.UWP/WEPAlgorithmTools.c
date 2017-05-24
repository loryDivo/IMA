#include "WEBPAlgorithmTools.h"

__declspec(dllexport)int WEBPEncode(char * imageSource, char * imageDestination) {
	return WEBPEncode_internal(imageSource, imageDestination);
}