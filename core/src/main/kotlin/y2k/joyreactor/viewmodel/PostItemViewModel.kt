package y2k.joyreactor.viewmodel

import y2k.joyreactor.model.Post
import y2k.joyreactor.services.PostService
import kotlin.reflect.KClass

/**
 * Created by y2k on 10/07/16.
 */
class PostItemViewModel(
    private val navigation: (KClass<*>, Any?) -> Unit,
    private val postService: PostService,
    val post: Post) {

    fun postClicked() {
        navigation(PostViewModel::class, post.id)
    }

    fun playClicked() {
        if (post.image?.isAnimated ?: false) navigation(VideoViewModel::class, post.id)
        else navigation(ImageViewModel::class, post.image!!.fullUrl())
    }

    fun changeLike() {
        navigation(PostLikeViewModel::class, "" + post.id)
    }

    fun toggleFavorite() {
        postService.toggleFavorite(post.id)
    }
}