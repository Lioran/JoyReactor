package y2k.joyreactor.viewmodel

import y2k.joyreactor.common.async.async_
import y2k.joyreactor.common.platform.NavigationService
import y2k.joyreactor.common.property
import y2k.joyreactor.services.requests.OriginalImageRequestFactory
import java.io.File

/**
 * Created by y2k on 3/8/16.
 */
class ImageViewModel(
    navigation: NavigationService,
    imageRequest: OriginalImageRequestFactory) {

    val isBusy = property(false)
    val imageFile = property<File>()

    init {
        async_ {
            isBusy += true
            try {
                imageFile += await(imageRequest(navigation.argument, false))
            } catch (e: Exception) {
                // TODO:
            }
            isBusy += false
        }
    }
}